namespace JTC.Data
{
    public class Database : DbContext
    {
        public DbSet<GuildEntity> Guilds { get; set; }

        public DbSet<HubEntity> Hubs { get; set; }

        public DbSet<TemporaryVoiceChannelEntity> TemporaryVoiceChannels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder) =>
            dbContextOptionsBuilder.UseSqlite($"Data Source={PathProvider.GetDatabasePath()}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuildEntity>()
                .HasKey(guild => guild.GuildId);
            modelBuilder.Entity<HubEntity>()
                .HasKey(hub => hub.VoiceChannelId);
            modelBuilder.Entity<TemporaryVoiceChannelEntity>()
                .HasKey(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId);
            modelBuilder.Entity<HubEntity>()
                .HasOne<GuildEntity>()
                .WithMany(guild => guild.Hubs)
                .HasForeignKey(hub => hub.GuildId)
                .HasPrincipalKey(guild => guild.GuildId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TemporaryVoiceChannelEntity>()
                .HasOne<HubEntity>()
                .WithMany(hub => hub.TemporaryVoiceChannels)
                .HasForeignKey(temporaryVoiceChannel => temporaryVoiceChannel.HubVoiceChannelId)
                .HasPrincipalKey(hub => hub.VoiceChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
