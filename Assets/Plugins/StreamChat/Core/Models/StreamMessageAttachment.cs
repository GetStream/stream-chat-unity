using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamMessageAttachment : IStateLoadableFrom<AttachmentInternalDTO, StreamMessageAttachment>
    {
        public System.Collections.Generic.List<StreamAttachmentAction> Actions { get; private set; }

        public string AssetUrl { get; private set; }

        public string AuthorIcon { get; private set; }

        public string AuthorLink { get; private set; }

        public string AuthorName { get; private set; }

        public string Color { get; private set; }

        public string Fallback { get; private set; }

        public System.Collections.Generic.List<StreamAttachmentField> Fields { get; private set; }

        public string Footer { get; private set; }

        public string FooterIcon { get; private set; }

        public StreamAttachmentImages Giphy { get; private set; }

        public string ImageUrl { get; private set; }

        public string OgScrapeUrl { get; private set; }

        public int? OriginalHeight { get; private set; }

        public int? OriginalWidth { get; private set; }

        public string Pretext { get; private set; }

        public string Text { get; private set; }

        public string ThumbUrl { get; private set; }

        public string Title { get; private set; }

        public string TitleLink { get; private set; }

        /// <summary>
        /// Attachment type (e.g. image, video, url)
        /// </summary>
        public string Type { get; private set; }

        StreamMessageAttachment IStateLoadableFrom<AttachmentInternalDTO, StreamMessageAttachment>.LoadFromDto(AttachmentInternalDTO dto, ICache cache)
        {
            Actions = Actions.TryLoadFromDtoCollection(dto.Actions, cache);
            AssetUrl = dto.AssetUrl;
            AuthorIcon = dto.AuthorIcon;
            AuthorLink = dto.AuthorLink;
            AuthorName = dto.AuthorName;
            Color = dto.Color;
            Fallback = dto.Fallback;
            Fields = Fields.TryLoadFromDtoCollection(dto.Fields, cache);
            Footer = dto.Footer;
            FooterIcon = dto.FooterIcon;
            Giphy = Giphy.TryLoadFromDto(dto.Giphy, cache);
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