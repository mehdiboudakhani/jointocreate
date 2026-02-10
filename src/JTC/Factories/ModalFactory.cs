namespace JTC.Factories
{
    public class ModalFactory
    {
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
