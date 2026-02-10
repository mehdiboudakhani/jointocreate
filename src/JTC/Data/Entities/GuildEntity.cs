namespace JTC.Data.Entities
{
    public class GuildEntity
    {
        public ulong GuildId { get; set; }
        public ICollection<HubEntity> Hubs { get; set; } = [];
    }
}
