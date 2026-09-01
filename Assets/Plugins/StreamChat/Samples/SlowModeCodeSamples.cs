using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Requests;
using StreamChat.Core.StatefulModels;
using UnityEngine;

namespace StreamChat.Samples
{
    internal sealed class SlowModeCodeSamples
    {
        /// <summary>
        /// https://getstream.io/chat/docs/unity/slow-mode/?language=unity#channel-slow-mode
        /// </summary>
        public async Task EnableDisableSlowMode()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");

// The Unity SDK has no dedicated slow mode method - set the channel's
// `cooldown` field, in seconds, with a partial channel update

// Enable slow mode with a 1s cooldown
            await channel.UpdatePartialAsync(new StreamUpdateChannelPartialRequest { Cooldown = 1 });

// Increase cooldown to 30s
            await channel.UpdatePartialAsync(new StreamUpdateChannelPartialRequest { Cooldown = 30 });

// Disable slow mode by setting the cooldown back to 0
            await channel.UpdatePartialAsync(new StreamUpdateChannelPartialRequest { Cooldown = 0 });

// Read the current cooldown. Null or 0 means slow mode is off
            Debug.Log(channel.Cooldown);
        }

        /// <summary>
        /// https://getstream.io/chat/docs/unity/slow-mode/?language=unity#channel-slow-mode
        /// </summary>
        public async Task GateSendingUiOnRemainingCooldown()
        {
            var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "channel-id");

// Disable/enable the send button based on the remaining cooldown
            var remaining = GetRemainingCooldown(channel);
            if (remaining > 0)
            {
                // disable/enable UI is app-specific
                DisableMessageSendingUi(remaining);
            }
        }

// The Unity SDK exposes the configured cooldown via `channel.Cooldown`, but does
// not track the remaining time for you. Compute it from the local user's last
// message in the channel

        private int GetRemainingCooldown(IStreamChannel channel)
        {
            var cooldown = channel.Cooldown ?? 0;
            if (cooldown <= 0)
            {
                return 0;
            }

            var localUserId = Client.LocalUserData.UserId;

            var lastOwnMessage = channel.Messages
                .Where(m => m.User != null && m.User.Id == localUserId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            if (lastOwnMessage == null)
            {
                return 0;
            }

            var elapsed = (DateTimeOffset.UtcNow - lastOwnMessage.CreatedAt).TotalSeconds;
            var remaining = cooldown - (int)elapsed;

            return remaining > 0 ? remaining : 0;
        }

        private void DisableMessageSendingUi(int forSeconds)
        {
        }

        private IStreamChatClient Client { get; } = StreamChatClient.CreateDefaultClient();
    }
}
