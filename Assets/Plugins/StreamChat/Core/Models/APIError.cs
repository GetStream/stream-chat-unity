using System;
using System.Text;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.Models;

namespace StreamChat.Core.Models
{
    public partial class APIError : ModelBase, ILoadableFrom<APIErrorDTO, APIError>
    {
        /// <summary>
        /// Response HTTP status code
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// API error code
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// Request duration
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Additional error info
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> ExceptionFields { get; set; }

        /// <summary>
        /// Message describing an error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// URL with additional information
        /// </summary>
        public string MoreInfo { get; set; }

        public void AppendFullLog(StringBuilder sb)
        {
            sb.Append(nameof(Message));
            sb.Append(": ");
            sb.Append(Message);
            sb.Append(" (More info below)");
            sb.Append(Environment.NewLine);

            sb.Append(nameof(StatusCode));
            sb.Append(": ");
            sb.Append(StatusCode);
            sb.Append(Environment.NewLine);

            sb.Append(nameof(Code));
            sb.Append(": ");
            sb.Append(Code);
            sb.Append(Environment.NewLine);

            sb.Append(nameof(MoreInfo));
            sb.Append(": ");
            sb.Append(MoreInfo);
            sb.Append(Environment.NewLine);

            if (ExceptionFields != null)
            {
                sb.Append(nameof(ExceptionFields));
                sb.Append(": ");
                sb.Append(Environment.NewLine);

                foreach (var entry in ExceptionFields)
                {
                    sb.Append(entry.Key);
                    sb.Append(": ");
                    sb.Append(entry.Value);
                    sb.Append(Environment.NewLine);
                }
            }
        }

        APIError ILoadableFrom<APIErrorDTO, APIError>.LoadFromDto(APIErrorDTO dto)
        {
            StatusCode = dto.StatusCode;
            Code = dto.Code;
            Duration = dto.Duration;
            ExceptionFields = dto.ExceptionFields;
            Message = dto.Message;
            MoreInfo = dto.MoreInfo;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}