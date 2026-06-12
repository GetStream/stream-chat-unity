using System;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Exceptions;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class RateLimitsCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/rate-limits/?language=unity#catching-429s-in-backend-sdks
        /// </summary>
        public async Task CatchingApiErrors()
        {
            IStreamChannel channel = null;

            // Example 1 — single catch + switch on the numeric error code.
            try
            {
                await channel.SendNewMessageAsync("Hello");
            }
            catch (StreamApiException ex)
            {
                // ex.Code / ex.StatusCode / ex.ErrorMessage / ex.ExceptionFields / ex.MoreInfo
                // are all available for inspection. See the API Error Codes docs page
                // for the full list.
                switch (ex.Code)
                {
                    case StreamApiException.RateLimitErrorStreamCode:
                        // HTTP 429 — back off with exponential delay before retrying.
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        break;
                    case StreamApiException.CooldownErrorStreamCode:
                        // HTTP 403 — slow-mode cooldown. Gate the send UI for `channel.Cooldown` seconds.
                        break;
                    case StreamApiException.PermissionDeniedErrorStreamCode:
                        // HTTP 403 — user lacks permission. Hide / disable the control.
                        break;
                    default:
                        Debug.LogError($"Stream API error {ex.Code} (HTTP {ex.StatusCode}): {ex.ErrorMessage}");
                        break;
                }
            }

            // Alternatively, use the `when` syntax combined with our dedicated
            // error-checking extensions.
            try
            {
                await channel.SendNewMessageAsync("Hello");
            }
            catch (StreamApiException ex) when (ex.IsRateLimitExceededError())
            {
                // HTTP 429 / Stream code 9 — back off before retrying.
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // Most common error cases have a dedicated extension method on StreamApiException.
            // Pick the helper that matches your branch instead of comparing `ex.Code` by hand:
            //   IsRateLimitExceededError       (429 / 9)     — back off and retry
            //   IsCooldownError                (403 / 60)    — gate UI for `channel.Cooldown` seconds
            //   IsPermissionDeniedError        (403 / 17)    — hide the UI control
            //   IsNoAccessToChannelsError      (403 / 70)    — drop the channel locally
            //   IsAppSuspendedError            (403 / 99)    — show service-unavailable UI
            //   IsAuthenticationError          (401 / 5)     — send the user back to sign-in
            //   IsTokenExpiredError            (401 / 40)    — refresh the token
            //   IsTokenError                   (401 / 40-43) — any token-related failure
            //   IsDoesNotExistError            (404 / 16)    — refresh the local view
            //   IsInputError                   (400 / 4)     — inspect `ex.ExceptionFields`
            //   IsMessageTooLongError          (400 / 20)    — show character-limit error
            //   IsMessageModerationFailedError (400 / 73)    — show "filtered" UI
            //   IsPayloadTooBigError           (413 / 22)    — ask for a smaller file
            //   IsInternalSystemError          (500 / -1)    — retry, then surface a transient-failure UI
            //
            // The general guidance: wrap a call in try/catch where you want to react
            // specifically (rate-limit back-off, cooldown UI, validation feedback,
            // permission gating). For everything else, let the exception propagate to
            // your global error handler — every SDK call already throws StreamApiException
            // on server-rejected requests, so a single boundary catch is enough.
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
