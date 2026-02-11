namespace JTC.Data.Entities
{
    /// <summary>
    /// Represents a hub in the database.
    /// </summary>
    public class HubEntity
    {
        /// <summary>
        /// The unique identifier of the hub's voice channel.
        /// </summary>
        public ulong VoiceChannelId { get; set; }

        /// <summary>
        /// The unique identifier of the guild that the hub belongs to.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Template name for the temporary voice channels created by this hub.
        /// </summary>
        public string ChildName { get; set; } = "{user}'s channel";

        /// <summary>
        /// Maximum number of users allowed in the temporary voice channels created by this hub. A value of 0 means no limit.
        /// </summary>
        public int UserLimit { get; set; } = 0;

        /// <summary>
        /// Whether the temporary voice channels created by this hub provide a management interface for the owner.
        /// </summary>
        public bool TemporaryVoiceChannelInterface { get; set; } = false;

        /// <summary>
        /// Collection of temporary voice channels created by this hub.
        /// </summary>
        public ICollection<TemporaryVoiceChannelEntity> TemporaryVoiceChannels { get; set; } = [];
    }
}
