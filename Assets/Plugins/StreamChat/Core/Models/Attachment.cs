using StreamChat.Core.DTO.Models;
using StreamChat.Core.Helpers;

namespace StreamChat.Core.Models
{
    public class Attachment : ModelBase, ILoadableFrom<AttachmentDTO, Attachment>
    {
        public System.Collections.Generic.ICollection<AttachmentAction> Actions { get; set; }

        public string AssetUrl { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorLink { get; set; }

        public string AuthorName { get; set; }

        public string Color { get; set; }

        public string Fallback { get; set; }

        public System.Collections.Generic.ICollection<Field> Fields { get; set; }

        public string Footer { get; set; }

        public string FooterIcon { get; set; }

        public Images Giphy { get; set; }

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

        Attachment ILoadableFrom<AttachmentDTO, Attachment>.LoadFromDto(AttachmentDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
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