using System.Collections.Generic;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.LowLevelClient;

namespace StreamChat.Core.Requests
{
    public class StreamAttachmentRequest : ISavableTo<AttachmentRequestInternalDTO>
    {
        public List<StreamActionRequest> Actions { get; set; }

        public string AssetUrl { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorLink { get; set; }

        public string AuthorName { get; set; }

        public string Color { get; set; }

        public string Fallback { get; set; }

        public List<StreamFieldRequest> Fields { get; set; }

        public string Footer { get; set; }

        public string FooterIcon { get; set; }

        public StreamImagesRequest Giphy { get; set; }

        public string ImageUrl { get; set; }

        public string OgScrapeUrl { get; set; }

        public int? OriginalHeight { get; set; }

        public int? OriginalWidth { get; set; }

        public string Pretext { get; set; }

        public string Text { get; set; }

        public string ThumbUrl { get; set; }

        public string Title { get; set; }

        public string TitleLink { get; set; }

        /// <summary>
        /// Attachment type (e.g. image, video, url)
        /// </summary>
        public string Type { get; set; }

        AttachmentRequestInternalDTO ISavableTo<AttachmentRequestInternalDTO>.SaveToDto() =>
            new AttachmentRequestInternalDTO
            {
                Actions = Actions.TrySaveToDtoCollection<StreamActionRequest, ActionRequestInternalDTO>(),
                AssetUrl = AssetUrl,
                AuthorIcon = AuthorIcon,
                AuthorLink = AuthorLink,
                AuthorName = AuthorName,
                Color = Color,
                Fallback = Fallback,
                Fields = Fields.TrySaveToDtoCollection<StreamFieldRequest, FieldRequestInternalDTO>(),
                Footer = Footer,
                FooterIcon = FooterIcon,
                Giphy = Giphy.TrySaveToDto(),
                ImageUrl = ImageUrl,
                OgScrapeUrl = OgScrapeUrl,
                OriginalHeight = OriginalHeight,
                OriginalWidth = OriginalWidth,
                Pretext = Pretext,
                Text = Text,
                ThumbUrl = ThumbUrl,
                Title = Title,
                TitleLink = TitleLink,
                Type = Type,
            };
    }
}