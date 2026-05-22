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
            int maxSeconds = 1000,
            string description = null,
            [CallerMemberName] string callerMember = null,
            [CallerFilePath] string callerFile = null,
            [CallerLineNumber] int callerLine = 0)
            => WaitForConditionAsync(condition, waitWhileTrue: true, maxSeconds, description, callerMember,
                callerFile, callerLine);

        protected static Task WaitWhileFalseAsync(Func<bool> condition,
            int maxSeconds = 1000,
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
        /// Timeout will be doubled on each subsequent attempt. So max timeout = <see cref="initTimeoutMs"/> * 2^<see cref="maxSeconds"/>
        /// </summary>
        protected static async Task<T> TryAsync<T>(Func<Task<T>> task, Predicate<T> successCondition,
            int maxSeconds = 1000,
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
                var response = await task();

                if (successCondition(response))
                {
                    return response;
                }

                if (sw.Elapsed.TotalSeconds > maxSeconds)
                {
                    throw new TimeoutException($"Timeout while waiting for {label}");
                }

                progress.MaybeLog(sw.Elapsed);

                var delay = (int)Math.Min(100 * 1000, Math.Pow(2, i + 9));
                await Task.Delay(delay);
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

                var delay = (int)Math.Min(100 * 1000, Math.Pow(2, i + 9));
                await Task.Delay(delay);
            }

            throw new TimeoutException($"Timeout while waiting for {label}");
        }

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
            const int maxAttempts = 7;
            var currentAttempt = 0;
            var completed = false;
            var exceptions = new List<Exception>();
            while (maxAttempts > currentAttempt)
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
                        var seconds = (int)Math.Max(1, Math.Min(60, Math.Pow(2, currentAttempt)));
                        await Task.Delay(1000 * seconds);
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