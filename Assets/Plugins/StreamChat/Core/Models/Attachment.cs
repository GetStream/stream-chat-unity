using StreamChat.Core.DTO.Models;

namespace StreamChat.Core.Models
{
    public partial class Attachment : ModelBase, ILoadableFrom<AttachmentDTO, Attachment>
    {
        public System.Collections.Generic.ICollection<ActionDTO> Actions { get; set; } //Todo: DTO -> Model

        public string AssetUrl { get; set; }

        public string AuthorIcon { get; set; }

        public string AuthorLink { get; set; }

        public string AuthorName { get; set; }

        public string Color { get; set; }

        public string Fallback { get; set; }

        public System.Collections.Generic.ICollection<FieldDTO> Fields { get; set; } //Todo: DTO -> Model

        public string Footer { get; set; }

        public string FooterIcon { get; set; }

        public ImagesDTO Giphy { get; set; }

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

        public Attachment LoadFromDto(AttachmentDTO dto)
        {
            AdditionalProperties = dto.AdditionalProperties;
            Actions = dto.Actions;
            AssetUrl = dto.AssetUrl;
            AuthorIcon = dto.AuthorIcon;
            AuthorLink = dto.AuthorLink;
            AuthorName = dto.AuthorName;
            Color = dto.Color;
            Fallback = dto.Fallback;
            Fields = dto.Fields;
            Footer = dto.Footer;
            FooterIcon = dto.FooterIcon;
            Giphy = dto.Giphy;
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