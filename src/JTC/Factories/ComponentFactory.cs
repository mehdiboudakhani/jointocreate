namespace JTC.Factories
{
    public class ComponentFactory
    {
        public ComponentBuilderV2 TemporaryVoiceChannelInterface(ulong voiceChannelId, ulong ownerId)
        {
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
            SelectMenuBuilder settings = new SelectMenuBuilder()
                .WithCustomId($"settings:{voiceChannelId}:{ownerId}")
                .WithPlaceholder("Settings...")
                .WithOptions([rename, userLimit, @lock, @unlock, kick]);
            ActionRowBuilder actionRow = new ActionRowBuilder()
                .WithSelectMenu(settings);
            SeparatorBuilder separator = new SeparatorBuilder()
                .WithIsDivider(true);
            TextDisplayBuilder title = new TextDisplayBuilder()
                .WithContent("🔊 **Temporary voice channel interface**");
            TextDisplayBuilder description = new TextDisplayBuilder()
                .WithContent("The owner can interact with this interface to configure the channel.");
            ContainerBuilder container = new ContainerBuilder()
                .WithComponents([title, separator, description, actionRow]);
            ComponentBuilderV2 component = new ComponentBuilderV2()
                .WithContainer(container);
            return component;
        }
    }
}
