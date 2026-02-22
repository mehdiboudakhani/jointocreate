namespace JTC.Data
{
    /// <summary>
    /// Represents the database context.
    /// </summary>
    /// <remarks>
    /// Configured to use a SQLite database.
    /// </remarks>
    public class Database : DbContext
    {
        /// <summary>
        /// DbSet of guilds in the database.
        /// </summary>
        public DbSet<GuildEntity> Guilds { get; set; }

        /// <summary>
        /// DbSet of hubs in the database.
        /// </summary>
        public DbSet<HubEntity> Hubs { get; set; }

        /// <summary>
        /// DbSet of temporary voice channels in the database.
        /// </summary>
        public DbSet<TemporaryVoiceChannelEntity> TemporaryVoiceChannels { get; set; }

        /// <summary>
        /// Configures the database context.
        /// </summary>
        /// <param name="dbContextOptionsBuilder">The builder used to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder) =>
            dbContextOptionsBuilder.UseSqlite($"Data Source={PathProvider.GetDatabasePath()}");

        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary keys
            modelBuilder.Entity<GuildEntity>()
                .HasKey(guild => guild.GuildId);
            modelBuilder.Entity<HubEntity>()
                .HasKey(hub => hub.VoiceChannelId);
            modelBuilder.Entity<TemporaryVoiceChannelEntity>()
                .HasKey(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId);

            // Relationships
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
