using System;
using System.Collections.Generic;
using GetStreamIO.Core.DTO.Requests;

namespace Plugins.GetStreamIO.Core.Requests.V2
{
    /// <summary>
    /// Represents chat user
    /// </summary>
    public class UserObjectRequest : RequestObjectBase, ISavableTo<UserObjectRequestDTO>
    {
        /// <summary>
        /// Expiration date of the ban
        /// </summary>
        public DateTimeOffset BanExpires { get; set; }

        /// <summary>
        /// Whether a user is banned or not
        /// </summary>
        public bool Banned { get; set; }

        /// <summary>
        /// Unique user identifier
        /// </summary>
        public string Id { get; set; }

        public bool Invisible { get; set; }

        /// <summary>
        /// Preferred language of a user
        /// </summary>
        public string Language { get; set; }

        public PushNotificationSettingsRequest PushNotifications { get; set; }

        /// <summary>
        /// Revocation date for tokens
        /// </summary>
        public DateTimeOffset RevokeTokensIssuedBefore { get; set; }

        /// <summary>
        /// Determines the set of user permissions
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// List of teams user is a part of
        /// </summary>
        public ICollection<string> Teams { get; set; }

        public UserObjectRequestDTO SaveToDto() =>
            new UserObjectRequestDTO
            {
                BanExpires = BanExpires,
                Banned = Banned,
                Id = Id,
                Invisible = Invisible,
                Language = Language,
                PushNotifications = PushNotifications.TrySaveToDto(),
                RevokeTokensIssuedBefore = RevokeTokensIssuedBefore,
                Role = Role,
                Teams = Teams,
                AdditionalProperties = AdditionalProperties
            };
    }
}