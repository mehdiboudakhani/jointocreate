namespace JTC.Factories
{
    /// <summary>
    /// Factory for creating modals.
    /// </summary>
    public class ModalFactory
    {
        /// <summary>
        /// Creates a modal for renaming a voice channel.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the voice channel.</param>
        /// <returns>A <see cref="ModalBuilder"/> representing the rename modal.</returns>
        public ModalBuilder RenameModal(ulong voiceChannelId)
        {
            TextInputBuilder textInput = new TextInputBuilder()
                .WithCustomId("name")
                .WithMinLength(1)
                .WithMaxLength(50);
            LabelBuilder label = new LabelBuilder()
                .WithLabel("Which name should this channel have?")
                .WithDescription("You can use the following variable: {user}.")
                .WithComponent(textInput);
            ModalBuilder modal = new ModalBuilder()
                .WithCustomId($"rename-modal:{voiceChannelId}")
                .WithTitle("Rename")
                .AddLabel(label);
            return modal;
        }

        /// <summary>
        /// Creates a modal for setting the user limit of a voice channel.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the voice channel.</param>
        /// <returns>A <see cref="ModalBuilder"/> representing the user limit modal.</returns>
        public ModalBuilder UserLimitModal(ulong voiceChannelId)
        {
            TextInputBuilder textInput = new TextInputBuilder()
                .WithCustomId("user-limit")
                .WithStyle(TextInputStyle.Short)
                .WithMinLength(1)
                .WithMaxLength(2);
            LabelBuilder label = new LabelBuilder()
                .WithLabel("What should the user limit be?")
                .WithDescription("The number must be between 0 and 99 (0 for unlimited).")
                .WithComponent(textInput);
            ModalBuilder modal = new ModalBuilder()
                .WithCustomId($"userlimit-modal:{voiceChannelId}")
                .WithTitle("User limit")
                .AddLabel(label);
            return modal;
        }

        /// <summary>
        /// Creates a modal for kicking a user from a voice channel.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the voice channel.</param>
        /// <param name="options">The list of selectable users to kick.</param>
        /// <returns>A <see cref="ModalBuilder"/> representing the kick modal.</returns>
        public ModalBuilder KickModal(ulong voiceChannelId, List<SelectMenuOptionBuilder> options)
        {
            ModalBuilder modal = new ModalBuilder()
                .WithCustomId($"kick-modal:{voiceChannelId}")
                .WithTitle("Kick")
                .AddSelectMenu("Which user do you want to kick?", "user", options);
            return modal;
        }
    }
}
