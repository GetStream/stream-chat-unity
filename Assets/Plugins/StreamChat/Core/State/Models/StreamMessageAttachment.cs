using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.State.Models
{
    public class StreamMessageAttachment : ILoadableFrom<AttachmentInternalDTO, StreamMessageAttachment>
    {
        public System.Collections.Generic.List<StreamAttachmentAction> Actions { get; set; }

        public string AssetUrl { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorLink { get; set; }

        public string AuthorName { get; set; }

        public string Color { get; set; }

        public string Fallback { get; set; }

        public System.Collections.Generic.List<StreamAttachmentField> Fields { get; set; }

        public string Footer { get; set; }

        public string FooterIcon { get; set; }

        public StreamAttachmentImages Giphy { get; set; }

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

        StreamMessageAttachment ILoadableFrom<AttachmentInternalDTO, StreamMessageAttachment>.LoadFromDto(AttachmentInternalDTO dto)
        {
            Actions = Actions.TryLoadFromDtoCollection(dto.Actions);
            AssetUrl = dto.AssetUrl;
            AuthorIcon = dto.AuthorIcon;
            AuthorLink = dto.AuthorLink;
            AuthorName = dto.AuthorName;
            Color = dto.Color;
            Fallback = dto.Fallback;
            Fields = Fields.TryLoadFromDtoCollection(dto.Fields);
            Footer = dto.Footer;
            FooterIcon = dto.FooterIcon;
            Giphy = Giphy.TryLoadFromDto(dto.Giphy);
            ImageUrl = dto.ImageUrl;
            OgScrapeUrl = dto.OgScrapeUrl;
            OriginalHeight = dto.OriginalHeight;
            OriginalWidth = dto.OriginalWidth;
            Pretext = dto.Pretext;
            Text = dto.Text;
            ThumbUrl = dto.ThumbUrl;
            Title = dto.Title;
            TitleLink = dto.TitleLink;
            Type = dto.Type;

            return this;
        }
    }
}