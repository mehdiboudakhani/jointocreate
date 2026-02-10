namespace JTC.Data.Entities
{
    public class HubEntity
    {
        public ulong VoiceChannelId { get; set; }
        public ulong GuildId { get; set; }
        public string ChildName { get; set; } = "{user}'s channel";
        public int UserLimit { get; set; } = 0;
        public bool TemporaryVoiceChannelInterface { get; set; } = false;
        public ICollection<TemporaryVoiceChannelEntity> TemporaryVoiceChannels { get; set; } = [];
    }
}
