namespace JTC.Data.Entities
{
    public class TemporaryVoiceChannelEntity
    {
        public ulong VoiceChannelId { get; set; }
        public ulong HubVoiceChannelId { get; set; }
        public ulong GuildId { get; set; }
        public ulong OwnerId { get; set; }
    }
}
