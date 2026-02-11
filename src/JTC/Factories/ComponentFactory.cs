namespace JTC.Factories
{
    /// <summary>
    /// Factory for creating components.
    /// </summary>
    public class ComponentFactory
    {
        /// <summary>
        /// Creates a temporary voice channel interface component.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="ownerId">The ID of the user who owns the channel.</param>
        /// <returns>A <see cref="ComponentBuilderV2"/> representing the interface.</returns>
        public ComponentBuilderV2 TemporaryVoiceChannelInterface(ulong voiceChannelId, ulong ownerId)
        {
            // Options for the select menu
            SelectMenuOptionBuilder rename = new SelectMenuOptionBuilder()
                .WithLabel("✏️ Rename")
                .WithDescription("Rename the channel.")
                .WithValue("rename");
            SelectMenuOptionBuilder userLimit = new SelectMenuOptionBuilder()
                .WithLabel("👥 User limit")
                .WithDescription("Define the maximum number of users.")
                .WithValue("userLimit");
            SelectMenuOptionBuilder @lock = new SelectMenuOptionBuilder()
                .WithLabel("🔒 Lock")
                .WithDescription("Lock the channel.")
                .WithValue("lock");
            SelectMenuOptionBuilder @unlock = new SelectMenuOptionBuilder()
                .WithLabel("🔓 Unlock")
                .WithDescription("Unlock the channel")
                .WithValue("unlock");
            SelectMenuOptionBuilder kick = new SelectMenuOptionBuilder()
                .WithLabel("👢 Kick")
                .WithDescription("Kick a user.")
                .WithValue("kick");

            // Select menu containing the options
            SelectMenuBuilder settings = new SelectMenuBuilder()
                .WithCustomId($"settings:{voiceChannelId}:{ownerId}")
                .WithPlaceholder("Settings...")
                .WithOptions([rename, userLimit, @lock, @unlock, kick]);

            // Action row containing the select menu
            ActionRowBuilder actionRow = new ActionRowBuilder()
                .WithSelectMenu(settings);

            // Separator and text components
            SeparatorBuilder separator = new SeparatorBuilder()
                .WithIsDivider(true);
            TextDisplayBuilder title = new TextDisplayBuilder()
                .WithContent("🔊 **Temporary voice channel interface**");
            TextDisplayBuilder description = new TextDisplayBuilder()
                .WithContent("The owner can interact with this interface to configure the channel.");

            // Container for all components
            ContainerBuilder container = new ContainerBuilder()
                .WithComponents([title, separator, description, actionRow]);

            // Final component
            ComponentBuilderV2 component = new ComponentBuilderV2()
                .WithContainer(container);

            return component;
        }
    }
}
