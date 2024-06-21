using StreamChat.Core.InternalDTO.Responses;

namespace StreamChat.Core.LowLevelClient.Responses
{
    public partial class SyncResponse : ResponseObjectBase, ILoadableFrom<SyncResponseInternalDTO, SyncResponse>
    {
        /// <summary>
        /// Duration of the request in human-readable format
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// List of events
        /// </summary>
        public System.Collections.Generic.List<object> Events { get; set; }

        /// <summary>
        /// List of CIDs that user can't access
        /// </summary>
        public System.Collections.Generic.List<string> InaccessibleCids { get; set; }

        SyncResponse ILoadableFrom<SyncResponseInternalDTO, SyncResponse>.LoadFromDto(SyncResponseInternalDTO dto)
        {
            Duration = dto.Duration;
            Events = dto.Events;
            InaccessibleCids = dto.InaccessibleCids;

            return this;
        }
    }
}