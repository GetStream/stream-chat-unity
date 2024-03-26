using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamDevice : IStateLoadableFrom<DeviceInternalDTO, StreamDevice>
    {
        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Whether device is disabled or not
        /// </summary>
        public bool? Disabled { get; private set; }

        /// <summary>
        /// Reason explaining why device had been disabled
        /// </summary>
        public string DisabledReason { get; private set; }

        /// <summary>
        /// Device ID
        /// </summary>
        public string Id { get; private set; }

        public PushProviderType? PushProvider { get; private set; }

        public string UserId { get; private set; }

        StreamDevice IStateLoadableFrom<DeviceInternalDTO, StreamDevice>.LoadFromDto(DeviceInternalDTO dto, ICache cache)
        {
            CreatedAt = dto.CreatedAt;
            Disabled = dto.Disabled;
            DisabledReason = dto.DisabledReason;
            Id = dto.Id;
            PushProvider = dto.PushProvider;
            UserId = dto.UserId;

            return this;
        }
    }
}