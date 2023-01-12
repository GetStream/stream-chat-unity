using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.Models
{
    public class StreamCommand : IStateLoadableFrom<CommandInternalDTO, StreamCommand>
    {
        /// <summary>
        /// Arguments help text, shown in commands auto-completion
        /// </summary>
        public string Args { get; private set; }

        /// <summary>
        /// Date/time of creation
        /// </summary>
        public System.DateTimeOffset? CreatedAt { get; private set; }

        /// <summary>
        /// Description, shown in commands auto-completion
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Unique command name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Set name used for grouping commands
        /// </summary>
        public string Set { get; private set; }

        /// <summary>
        /// Date/time of the last update
        /// </summary>
        public System.DateTimeOffset? UpdatedAt { get; private set; }

        StreamCommand IStateLoadableFrom<CommandInternalDTO, StreamCommand>.LoadFromDto(CommandInternalDTO dto, ICache cache)
        {
            Args = dto.Args;
            CreatedAt = dto.CreatedAt;
            Description = dto.Description;
            Name = dto.Name;
            Set = dto.Set;
            UpdatedAt = dto.UpdatedAt;

            return this;
        }
    }
}