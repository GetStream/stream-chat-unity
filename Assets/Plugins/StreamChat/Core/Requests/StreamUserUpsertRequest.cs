using System;
using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    public sealed class StreamUserUpsertRequest : ISavableTo<UserObjectRequestInternalDTO>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public DateTimeOffset? BanExpires { get; set; }

        /// <summary>
        /// Whether a user is banned or not
        /// </summary>
        public bool? Banned { get; set; }

        /// <summary>
        /// Unique user identifier
        /// </summary>
        public string Id { get; set; }

        public bool? Invisible { get; set; }

        /// <summary>
        /// Preferred language of a user
        /// </summary>
        public string Language { get; set; }

        public StreamPushNotificationSettingsRequest PushNotifications { get; set; }

        /// <summary>
        /// Revocation date for tokens
        /// </summary>
        public DateTimeOffset? RevokeTokensIssuedBefore { get; set; }

        /// <summary>
        /// Determines the set of user permissions
        /// </summary>
        public string Role { get; set; } //StreamTodo: change to enum?

        /// <summary>
        /// List of teams user is a part of
        /// </summary>
        public List<string> Teams { get; set; }

        public string Name;
        public string Image;

        /// <summary>
        /// Any custom data to associate with this message. This will be accessible through <see cref="IStreamMessage.CustomData"/>
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        UserObjectRequestInternalDTO ISavableTo<UserObjectRequestInternalDTO>.SaveToDto()
            => new UserObjectRequestInternalDTO
            {
                BanExpires = BanExpires,
                Banned = Banned,
                Id = Id,
                Invisible = Invisible,
                Language = Language,
                PushNotifications = PushNotifications?.TrySaveToDto(),
                RevokeTokensIssuedBefore = RevokeTokensIssuedBefore,
                Role = Role,
                Teams = Teams,
                Name = Name,
                Image = Image,
                AdditionalProperties = CustomData?.ToDictionary()
            };
    }
}