#if STREAM_TESTS_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using StreamChat.Libs.Auth;
using Debug = UnityEngine.Debug;

namespace StreamChat.Tests.StatefulClient
{
    internal abstract class BaseStateIntegrationTests
    {
        [OneTimeSetUp]
        public void OneTimeUp()
        {
            Debug.Log("------------ Up");
            StreamTestClients.Instance.AddLock(this);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Debug.Log("------------ TearDown");

            // NUnit drives an `async Task` OneTimeTearDown by blocking the main thread on the
            // returned task (effectively `task.GetAwaiter().GetResult()`). Any `await` inside
            // captures Unity's UnitySynchronizationContext and posts its continuation back to
            // the main thread, which is the very thread NUnit is blocking - classic async-over-
            // sync deadlock. Symptom: `Debug.Log("------------ TearDown")` is the last log line
            // in Editor.log and Unity hangs with no further output (the kicked-off DELETE
            // /channels HTTP call completes, but its continuation never gets to resume).
            //
            // We can't go back to `async void` (NUnit rejects it with `ArgumentException:
            // 'async void' methods are not supported`). Hopping the cleanup onto the thread
            // pool detaches it from the Unity SynchronizationContext, so the awaited
            // continuations resume on thread-pool threads and the main thread is only
            // blocked waiting for a task that no longer needs it.
            Task.Run(async () =>
            {
                await DeleteTempChannelsAsync();
                await StreamTestClients.Instance.RemoveLockAsync(this);
            }).GetAwaiter().GetResult();
        }

        protected static StreamChatClient Client => StreamTestClients.Instance.StateClient;

        protected int MainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;
        
        protected AuthCredentials AdminPrimaryCredentials => StreamTestClients.Instance.AdminPrimaryCredentials;
        protected AuthCredentials AdminSecondaryCredentials => StreamTestClients.Instance.AdminSecondaryCredentials;
        
        protected AuthCredentials UserPrimaryCredentials => StreamTestClients.Instance.UserPrimaryCredentials;
        protected AuthCredentials UserSecondaryCredentials => StreamTestClients.Instance.UserSecondaryCredentials;

        protected int GetCurrentThreadId() => Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// Create temp channel with random id that will be removed in [TearDown]
        /// </summary>
        protected async Task<IStreamChannel> CreateUniqueTempChannelAsync(string name = null, bool watch = true, ChannelType? channelTypeOverride = default, StreamChatClient overrideClient = null)
        {
            var channelId = "random-channel-11111-" + Guid.NewGuid();
            var client = overrideClient ?? Client;
            var channelType = channelTypeOverride ?? ChannelType.Messaging;

            var channelState = await client.InternalGetOrCreateChannelWithIdAsync(channelType, channelId, name, watch: watch);
            _tempChannels.Add(channelState);
            return channelState;
        }

        /// <summary>
        /// Create temp user with random id
        /// </summary>
        protected async Task<IStreamUser> CreateUniqueTempUserAsync(string name, string prefix = "")
        {
            var userId = prefix + "random-user-22222-" + Guid.NewGuid() + "-" + name;

            var user = await Client.UpsertUsers(new StreamUserUpsertRequest[]
            {
                new StreamUserUpsertRequest
                {
                    Id = userId,
                    Name = name
                }
            });
            return user.First();
        }

        /// <summary>
        /// Use only if you've successfully deleted the channel
        /// </summary>
        protected void SkipThisTempChannelDeletionInTearDown(IStreamChannel channel)
        {
            _tempChannels.Remove(channel);
        }

        protected static IEnumerator ConnectAndExecute(Func<Task> test)
        {
            yield return ConnectAndExecuteAsync(test).RunAsIEnumerator();
        }

        protected Task<StreamChatClient> GetConnectedOtherClientAsync()
            => StreamTestClients.Instance.ConnectOtherStateClientAsync();

        /// <summary>
        /// Timeout for <see cref="WaitWhileTrueAsync"/> / <see cref="WaitWhileFalseAsync"/>.
        /// Those helpers only inspect local client state (for example waiting until a websocket
        /// event updates a channel after a REST call). They do not send HTTP, so they do not
        /// need the long rate-limit budget used by <see cref="ApiCallWaitSeconds"/>.
        /// </summary>
        protected const int WebsocketEventWaitSeconds = 60;

        /// <summary>
        /// Timeout for <see cref="TryAsync"/>, which repeats a Stream API call until a condition
        /// holds. Several CI jobs share one Stream app, so a 429 can pause a single request for
        /// a minute or more. Keep this large so jobs wait each other out instead of failing.
        /// </summary>
        protected const int ApiCallWaitSeconds = 1000;

        /// <summary>
        /// Timeout for HTTP 500 retries (e.g. "query channels timed out"). Short on purpose:
        /// this is a transient backend error, not a rate-limit collision between CI jobs.
        /// </summary>
        protected const int TransientApiErrorWaitSeconds = 60;

        //StreamTodo: figure out syntax to wrap call in using that will subscribe to observing an event if possible
        /// <summary>
        /// Use this if state update depends on receiving WS event that might come after the REST call was completed.
        /// </summary>
        /// <param name="condition">Returns true while we should keep waiting. Returns false to break the wait.</param>
        /// <param name="description">
        /// Optional human description of what we are waiting for. Surfaces in periodic "still waiting" logs
        /// and timeout messages, so a hanging test can be diagnosed from logs alone without grepping line numbers.
        /// </param>
        protected static Task WaitWhileTrueAsync(Func<bool> condition,
            int maxSeconds = WebsocketEventWaitSeconds,
            string description = null,
            [CallerMemberName] string callerMember = null,
            [CallerFilePath] string callerFile = null,
            [CallerLineNumber] int callerLine = 0)
            => WaitForConditionAsync(condition, waitWhileTrue: true, maxSeconds, description, callerMember,
                callerFile, callerLine);

        protected static Task WaitWhileFalseAsync(Func<bool> condition,
            int maxSeconds = WebsocketEventWaitSeconds,
            string description = null,
            [CallerMemberName] string callerMember = null,
            [CallerFilePath] string callerFile = null,
            [CallerLineNumber] int callerLine = 0)
            => WaitForConditionAsync(condition, waitWhileTrue: false, maxSeconds, description, callerMember,
                callerFile, callerLine);

        protected static async Task WaitWithTimeoutAsync(Task task, string exceptionMsg, int maxSeconds = 300)
        {
            if (await Task.WhenAny(task, Task.Delay(maxSeconds * 1000)) != task)
            {
                throw new TimeoutException(exceptionMsg);
            }
        }

        /// <summary>
        /// Repeat <paramref name="task"/> until <paramref name="successCondition"/> holds.
        /// Rate-limit (429) retries use <see cref="ApiCallWaitSeconds"/> and a long backoff.
        /// HTTP 500 retries use <see cref="TransientApiErrorWaitSeconds"/>.
        /// </summary>
        protected static async Task<T> TryAsync<T>(Func<Task<T>> task, Predicate<T> successCondition,
            int maxSeconds = ApiCallWaitSeconds,
            string description = null,
            [CallerMemberName] string callerMember = null,
            [CallerFilePath] string callerFile = null,
            [CallerLineNumber] int callerLine = 0)
        {
            var sw = new Stopwatch();
            sw.Start();
            var label = BuildWaitLabel(description, callerMember, callerFile, callerLine);
            var progress = new WaitProgressLogger(label);

            for (int i = 0; i < int.MaxValue; i++)
            {
                T response;
                try
                {
                    response = await task();
                }
                catch (StreamApiException e) when (e.IsRateLimitExceededError())
                {
                    if (sw.Elapsed.TotalSeconds > ApiCallWaitSeconds)
                    {
                        throw new TimeoutException($"Timeout while waiting for {label}", e);
                    }

                    progress.MaybeLog(sw.Elapsed);
                    await Task.Delay(RateLimitBackoffMs(i));
                    continue;
                }
                catch (StreamApiException e) when (e.IsInternalSystemError())
                {
                    if (sw.Elapsed.TotalSeconds > TransientApiErrorWaitSeconds)
                    {
                        throw new TimeoutException($"Timeout while waiting for {label}", e);
                    }

                    progress.MaybeLog(sw.Elapsed);
                    await Task.Delay(TransientErrorBackoffMs(i));
                    continue;
                }

                if (successCondition(response))
                {
                    return response;
                }

                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException($"Timeout while waiting for {label}");
                }

                progress.MaybeLog(sw.Elapsed);
                await Task.Delay(BackoffDelayMs(i));
            }

            throw new TimeoutException($"Timeout while waiting for {label}");
        }

        private static async Task WaitForConditionAsync(Func<bool> condition, bool waitWhileTrue, int maxSeconds,
            string description, string callerMember, string callerFile, int callerLine)
        {
            var sw = new Stopwatch();
            sw.Start();
            var label = BuildWaitLabel(description, callerMember, callerFile, callerLine);
            var progress = new WaitProgressLogger(label);

            for (int i = 0; i < int.MaxValue; i++)
            {
                var value = condition();
                var shouldStop = waitWhileTrue ? !value : value;
                if (shouldStop)
                {
                    return;
                }

                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException($"Timeout while waiting for {label}");
                }

                progress.MaybeLog(sw.Elapsed);
                await Task.Delay(BackoffDelayMs(i));
            }

            throw new TimeoutException($"Timeout while waiting for {label}");
        }

        private static int BackoffDelayMs(int attempt)
            => (int)Math.Min(100 * 1000, Math.Pow(2, attempt + 9));

        // Matches InternalApiClientBase test-mode 429 backoff (61s, then 81s, 101s, …).
        private static int RateLimitBackoffMs(int attempt)
            => (61 + attempt * 20) * 1000;

        private static int TransientErrorBackoffMs(int attempt)
            => (int)Math.Min(16 * 1000, Math.Pow(2, attempt) * 1000);

        private static string BuildWaitLabel(string description, string callerMember, string callerFile, int callerLine)
        {
            var fileName = string.IsNullOrEmpty(callerFile) ? "<unknown>" : Path.GetFileName(callerFile);
            if (!string.IsNullOrEmpty(description))
            {
                return $"\"{description}\" ({callerMember} @ {fileName}:{callerLine})";
            }

            return $"condition at {callerMember} @ {fileName}:{callerLine}";
        }

        // Emits one log line per crossed elapsed-time threshold (2m, 5m, 10m, 20m, 30m) so a hanging
        // wait is visible in the test output without needing per-test instrumentation. Single instance
        // per Wait* call, so each threshold fires at most once for that wait.
        private sealed class WaitProgressLogger
        {
            private static readonly TimeSpan[] Thresholds =
            {
                TimeSpan.FromMinutes(0.5),
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(2),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMinutes(20),
                TimeSpan.FromMinutes(30),
            };

            private readonly string _label;
            private int _nextIndex;

            public WaitProgressLogger(string label)
            {
                _label = label;
                _nextIndex = 0;
            }

            public void MaybeLog(TimeSpan elapsed)
            {
                while (_nextIndex < Thresholds.Length && elapsed >= Thresholds[_nextIndex])
                {
                    var threshold = Thresholds[_nextIndex];
                    Debug.Log(
                        $"[StreamChatTests] still waiting for {_label} after {FormatDuration(threshold)} (elapsed: {FormatDuration(elapsed)})");
                    _nextIndex++;
                }
            }

            private static string FormatDuration(TimeSpan ts)
            {
                if (ts.TotalMinutes >= 1)
                {
                    return ts.Seconds == 0
                        ? $"{(int)ts.TotalMinutes}m"
                        : $"{(int)ts.TotalMinutes}m{ts.Seconds:D2}s";
                }

                return $"{(int)ts.TotalSeconds}s";
            }
        }

        private readonly List<IStreamChannel> _tempChannels = new List<IStreamChannel>();

        private static async Task ConnectAndExecuteAsync(Func<Task> test)
        {
            await StreamTestClients.Instance.ConnectStateClientAsync();
            const int maxTransientAttempts = 7;
            const int maxRateLimitAttempts = 20;
            var currentAttempt = 0;
            var completed = false;
            var exceptions = new List<Exception>();
            while (true)
            {
                currentAttempt++;
                try
                {
                    await test();
                    completed = true;
                    break;
                }
                catch (StreamApiException e)
                {
                    exceptions.Add(e);
                    if (e.IsRateLimitExceededError())
                    {
                        if (currentAttempt >= maxRateLimitAttempts)
                        {
                            break;
                        }

                        await Task.Delay(RateLimitBackoffMs(currentAttempt - 1));
                        continue;
                    }

                    if (e.IsInternalSystemError())
                    {
                        if (currentAttempt >= maxTransientAttempts)
                        {
                            break;
                        }

                        await Task.Delay(TransientErrorBackoffMs(currentAttempt - 1));
                        continue;
                    }

                    throw;
                }
            }

            if (!completed)
            {
                throw new AggregateException($"Failed all attempts. Last Exception: {exceptions.Last().Message} ", exceptions);
            }
        }
        
        private async Task DeleteTempChannelsAsync()
        {
            if (_tempChannels.Count == 0)
            {
                return;
            }

            await Client.DeleteMultipleChannelsAsync(_tempChannels, isHardDelete: true);

            _tempChannels.Clear();
        }
    }
}
#endif