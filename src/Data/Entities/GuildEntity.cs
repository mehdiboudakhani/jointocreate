namespace JTC.Data.Entities
{
    /// <summary>
    /// Represents a guild in the database.
    /// </summary>
    public class GuildEntity
    {
        /// <summary>
        /// The unique identifier of the guild.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Collection of hubs associated with the guild.
        /// </summary>
        public ICollection<HubEntity> Hubs { get; set; } = [];
    }
}
