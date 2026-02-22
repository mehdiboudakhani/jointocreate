namespace JTC.Events
{
    /// <summary>
    /// Handles events related to Discord modals.
    /// </summary>
    /// <param name="discordSocketClient">The Discord client used to access voice channels.</param>
    public class ModalEvent(DiscordSocketClient discordSocketClient)
    {
        /// <summary>
        /// Called when a Discord modal is submitted.
        /// Routes to the appropriate handler based on the modal type.
        /// </summary>
        /// <param name="socketModal">The submitted modal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnModalSubmitted(SocketModal socketModal)
        {
            string[] parts = socketModal.Data.CustomId.Split(':');
            if (parts.Length is not 2 || !ulong.TryParse(parts[1], out var voiceChannelId))
                return;
            var components = socketModal.Data.Components.ToDictionary(
                component => component.CustomId,
                component => component.Values?.FirstOrDefault() ?? component.Value);
            switch (parts[0])
            {
                case "rename-modal":
                    await RenameTemporaryVoiceChannelAsync(socketModal, voiceChannelId, components);
                    break;
                case "kick-modal":
                    await KickTemporaryVoiceChannelAsync(socketModal, voiceChannelId, components);
                    break;
                case "userlimit-modal":
                    await UserLimitTemporaryVoiceChannelAsync(socketModal, voiceChannelId, components);
                    break;
            }
        }

        /// <summary>
        /// Renames a temporary voice channel based on modal input.
        /// </summary>
        /// <param name="socketModal">The modal submitted.</param>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="data">Dictionary of input values from the modal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task RenameTemporaryVoiceChannelAsync(SocketModal socketModal, ulong voiceChannelId, Dictionary<string, string> data)
        {
            if (data.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name) && GetVoiceChannel(voiceChannelId) is { } voiceChannel)
            {
                string finalName = name.Trim().Replace("{user}", socketModal.User as SocketGuildUser is not null
                    ? ((SocketGuildUser)socketModal.User).DisplayName
                    : socketModal.User.Username);
                await voiceChannel.ModifyAsync(props => props.Name = finalName);
                await socketModal.RespondAsync($"The channel has been renamed to {finalName}.", ephemeral: true);
            }
        }

        /// <summary>
        /// Kicks a user from a temporary voice channel based on modal input.
        /// </summary>
        /// <param name="socketModal">The modal submitted.</param>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="data">Dictionary of input values from the modal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task KickTemporaryVoiceChannelAsync(SocketModal socketModal, ulong voiceChannelId, Dictionary<string, string> data)
        {
            if (data.TryGetValue("user", out var userIdString) && ulong.TryParse(userIdString, out var userId) && GetVoiceChannel(voiceChannelId) is { } voiceChannel && voiceChannel.ConnectedUsers.FirstOrDefault(user => user.Id == userId) is { } kicked)
            {
                await kicked.ModifyAsync(user => user.Channel = null);
                await socketModal.RespondAsync($"The user {kicked.DisplayName} has been kicked.", ephemeral: true);
            }
        }

        /// <summary>
        /// Sets the user limit for a temporary voice channel based on modal input.
        /// </summary>
        /// <param name="socketModal">The modal submitted.</param>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="data">Dictionary of input values from the modal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UserLimitTemporaryVoiceChannelAsync(SocketModal socketModal, ulong voiceChannelId, Dictionary<string, string> data)
        {
            if (!data.TryGetValue("user-limit", out var userLimitString) || !int.TryParse(userLimitString, out var userLimit) || userLimit < 0 || userLimit > 99)
            {
                await socketModal.RespondAsync("The user limit must be between 0 and 99.", ephemeral: true);
                return;
            }
            var voiceChannel = GetVoiceChannel(voiceChannelId);
            if (voiceChannel is null)
                return;
            await voiceChannel.ModifyAsync(props => props.UserLimit = userLimit);
            await socketModal.RespondAsync($"The user limit has been set to {userLimit}.", ephemeral: true);
        }

        /// <summary>
        /// Retrieves a voice channel by its ID.
        /// </summary>
        /// <param name="channelId">The ID of the voice channel.</param>
        /// <returns>The voice channel if found; otherwise, null.</returns>
        private SocketVoiceChannel? GetVoiceChannel(ulong channelId) =>
            discordSocketClient.GetChannel(channelId) as SocketVoiceChannel;
    }
}
