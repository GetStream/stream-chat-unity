using System.Collections.Generic;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.Requests
{
    /// <summary>
    /// Partial update for <see cref="IStreamChannel.UpdatePartialAsync"/>.
    /// Only properties you set are sent; everything else on the channel is kept.
    /// This is the opposite of <see cref="StreamUpdateOverwriteChannelRequest"/> used by
    /// <see cref="IStreamChannel.UpdateOverwriteAsync"/>, where omitted fields (including
    /// <see cref="CustomData"/>) are deleted.
    /// Custom data is flattened into the PUT <c>set</c> bag as top-level keys, matching the Chat API.
    /// </summary>
    public sealed class StreamUpdateChannelPartialRequest
    {
        /// <summary>
        /// Enable or disable auto translation. Null = leave unchanged.
        /// </summary>
        public bool? AutoTranslationEnabled { get; set; }

        /// <summary>
        /// Auto translation language (ISO code). Null = leave unchanged.
        /// </summary>
        public string AutoTranslationLanguage { get; set; }

        /// <summary>
        /// Slow-mode cooldown in seconds. Null = leave unchanged. Set to 0 to disable slow mode.
        /// </summary>
        public int? Cooldown { get; set; }

        /// <summary>
        /// Disable or enable the channel. Null = leave unchanged.
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// Freeze or unfreeze the channel. Null = leave unchanged.
        /// </summary>
        public bool? Frozen { get; set; }

        /// <summary>
        /// Channel name. Null = leave unchanged.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Team the channel belongs to (multi-tenant). Null = leave unchanged.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Custom keys to set. Flattened into <c>set</c> next to reserved fields (not a nested object).
        /// Null or empty = do not change existing custom data. Unlike overwrite, this does not wipe keys
        /// you omit.
        /// </summary>
        public StreamCustomDataRequest CustomData { get; set; }

        /// <summary>
        /// Field names to remove (reserved or custom). Empty is ignored when something is being set.
        /// </summary>
        public IEnumerable<string> Unset { get; set; }

        internal Dictionary<string, object> ToSetDictionary()
        {
            var set = new Dictionary<string, object>();

            if (CustomData != null)
            {
                foreach (var kvp in CustomData.ToDictionary())
                {
                    set[kvp.Key] = kvp.Value;
                }
            }

            if (AutoTranslationEnabled.HasValue)
            {
                set["auto_translation_enabled"] = AutoTranslationEnabled.Value;
            }

            if (AutoTranslationLanguage != null)
            {
                set["auto_translation_language"] = AutoTranslationLanguage;
            }

            if (Cooldown.HasValue)
            {
                set["cooldown"] = Cooldown.Value;
            }

            if (Disabled.HasValue)
            {
                set["disabled"] = Disabled.Value;
            }

            if (Frozen.HasValue)
            {
                set["frozen"] = Frozen.Value;
            }

            if (Name != null)
            {
                set["name"] = Name;
            }

            if (Team != null)
            {
                set["team"] = Team;
            }

            return set;
        }
    }
}
