using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;

namespace StreamChat.Core.Requests
{
    public partial class ChannelConfigRequest : RequestObjectBase, ISavableTo<ChannelConfigRequestDTO>
    {
        public string Blocklist { get; set; }

        public AutomodBehaviourType? BlocklistBehavior { get; set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> Grants { get; set; }

        /// <summary>
        /// Overrides max message length
        /// </summary>
        public int? MaxMessageLength { get; set; }

        /// <summary>
        /// Enables message quotes
        /// </summary>
        public bool? Quotes { get; set; }

        /// <summary>
        /// Enables or disables reactions
        /// </summary>
        public bool? Reactions { get; set; }

        /// <summary>
        /// Enables message replies (threads)
        /// </summary>
        public bool? Replies { get; set; }

        /// <summary>
        /// Enables or disables typing events
        /// </summary>
        public bool? TypingEvents { get; set; }

        /// <summary>
        /// Enables or disables file uploads
        /// </summary>
        public bool? Uploads { get; set; }

        /// <summary>
        /// Enables or disables URL enrichment
        /// </summary>
        public bool? UrlEnrichment { get; set; }

        ChannelConfigRequestDTO ISavableTo<ChannelConfigRequestDTO>.SaveToDto() =>
            new ChannelConfigRequestDTO
            {
                Blocklist = Blocklist,
                BlocklistBehavior = BlocklistBehavior,
                Grants = Grants,
                MaxMessageLength = MaxMessageLength,
                Quotes = Quotes,
                Reactions = Reactions,
                Replies = Replies,
                TypingEvents = TypingEvents,
                Uploads = Uploads,
                UrlEnrichment = UrlEnrichment,
                AdditionalProperties = AdditionalProperties
            };
    }
}