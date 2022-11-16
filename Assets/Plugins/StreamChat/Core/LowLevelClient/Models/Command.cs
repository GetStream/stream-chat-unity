using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.LowLevelClient.Models
{
    public class Command : ModelBase, ILoadableFrom<CommandInternalDTO, Command>
    {
        /// <summary>
        /// Arguments help text, shown in commands auto-completion
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Description, shown in commands auto-completion
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Unique command name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set name used for grouping commands
        /// </summary>
        public string Set { get; set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; set; }

        Command ILoadableFrom<CommandInternalDTO, Command>.LoadFromDto(CommandInternalDTO dto)
        {
            Args = dto.Args;
            CreatedAt = dto.CreatedAt;
            Description = dto.Description;
            Name = dto.Name;
            Set = dto.Set;
            UpdatedAt = dto.UpdatedAt;
            AdditionalProperties = dto.AdditionalProperties;

            return this;
        }
    }
}