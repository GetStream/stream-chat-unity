using StreamChat.Core.InternalDTO.Events;

namespace StreamChat.Core.Helpers
{
    /// <summary>
    /// This class fixes the case where sometimes response contains empty channel.id and channel.type but instead has channel_id and channel_type
    /// </summary>
    internal static class InternalNotificationsHelper
    {
        public static void FixMissingChannelTypeAndId(NotificationAddedToChannelEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationChannelDeletedEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationChannelTruncatedEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationInviteAcceptedEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationInvitedEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationInviteRejectedEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        public static void FixMissingChannelTypeAndId(NotificationNewMessageEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }

        public static void FixMissingChannelTypeAndId(NotificationRemovedFromChannelEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }        
        
        public static void FixMissingChannelTypeAndId(NotificationMarkReadEventInternalDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Channel.Id))
            {
                dto.Channel.Id = dto.ChannelId;
            }

            if (string.IsNullOrEmpty(dto.Channel.Type))
            {
                dto.Channel.Type = dto.ChannelType;
            }
        }
        
        
    }
}