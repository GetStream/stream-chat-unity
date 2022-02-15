using System.Collections.Generic;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Requests
{
    public class AttachmentRequest : RequestObjectBase, ISavableTo<AttachmentRequestDTO>
    {
        public ICollection<ActionRequest> Actions { get; set; }

        public string AssetUrl { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorLink { get; set; }

        public string AuthorName { get; set; }

        public string Color { get; set; }

        public string Fallback { get; set; }

        public ICollection<FieldRequest> Fields { get; set; }

        public string Footer { get; set; }

        public string FooterIcon { get; set; }

        public ImagesRequest Giphy { get; set; }

        public string ImageUrl { get; set; }

        public string OgScrapeUrl { get; set; }

        public string Pretext { get; set; }

        public string Text { get; set; }

        public string ThumbUrl { get; set; }

        public string Title { get; set; }

        public string TitleLink { get; set; }

        /// <summary>
        /// Attachment type (e.g. image, video, url)
        /// </summary>
        public string Type { get; set; }

        AttachmentRequestDTO ISavableTo<AttachmentRequestDTO>.SaveToDto() =>
            new AttachmentRequestDTO
            {
                Actions = Actions.TrySaveToDtoCollection<ActionRequest, ActionRequestDTO>(),
                AssetUrl = AssetUrl,
                AuthorIcon = AuthorIcon,
                AuthorLink = AuthorLink,
                AuthorName = AuthorName,
                Color = Color,
                Fallback = Fallback,
                Fields = Fields.TrySaveToDtoCollection<FieldRequest, FieldRequestDTO>(),
                Footer = Footer,
                FooterIcon = FooterIcon,
                Giphy = Giphy.TrySaveToDto(),
                ImageUrl = ImageUrl,
                OgScrapeUrl = OgScrapeUrl,
                Pretext = Pretext,
                Text = Text,
                ThumbUrl = ThumbUrl,
                Title = Title,
                TitleLink = TitleLink,
                Type = Type,
                AdditionalProperties = AdditionalProperties
            };
    }
}