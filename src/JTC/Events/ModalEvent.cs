namespace JTC.Events
{
    public class ModalEvent(DiscordSocketClient discordSocketClient)
    {
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

        private async Task KickTemporaryVoiceChannelAsync(SocketModal socketModal, ulong voiceChannelId, Dictionary<string, string> data)
        {
            if (data.TryGetValue("user", out var userIdString) && ulong.TryParse(userIdString, out var userId) && GetVoiceChannel(voiceChannelId) is { } voiceChannel && voiceChannel.ConnectedUsers.FirstOrDefault(user => user.Id == userId) is { } kicked)
            {
                await kicked.ModifyAsync(user => user.Channel = null);
                await socketModal.RespondAsync($"The user {kicked.DisplayName} has been kicked.", ephemeral: true);
            }
        }

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

        private SocketVoiceChannel? GetVoiceChannel(ulong channelId) =>
            discordSocketClient.GetChannel(channelId) as SocketVoiceChannel;
    }
}
