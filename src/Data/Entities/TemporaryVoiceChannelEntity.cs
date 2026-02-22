namespace JTC.Data.Entities
{
    /// <summary>
    /// Represents a temporary voice channel in the database.
    /// </summary>
    public class TemporaryVoiceChannelEntity
    {
        /// <summary>
        /// The unique identifier of the temporary voice channel.
        /// </summary>
        public ulong VoiceChannelId { get; set; }

        /// <summary>
        /// The voice channel ID of the hub that created this temporary voice channel.
        /// </summary>
        public ulong HubVoiceChannelId { get; set; }

        /// <summary>
        /// The ID of the guild this temporary voice channel belongs to.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// The ID of the user who owns this temporary voice channel.
        /// </summary>
        public ulong OwnerId { get; set; }
    }
}
