namespace JTC.Interactions
{
    public class TemporaryVoiceChannelInteraction(
        DiscordSocketClient discordSocketClient, 
        ModalFactory modalFactory, 
        TemporaryVoiceChannelService temporaryVoiceChannelService) : InteractionModuleBase<SocketInteractionContext>
    {
        [ComponentInteraction("settings:*")]
        public async Task SettingsAsync(string rawData, string[] values)
        {
            var parts = rawData.Split(':');
            if (parts.Length != 2)
                return;
            if (!ulong.TryParse(parts[0], out var voiceChannelId))
                return;
            if (!await temporaryVoiceChannelService.IsOwnerAsync(voiceChannelId, Context.User.Id))
            {
                await RespondAsync("Only the channel owner can use this interface.", ephemeral: true);
                return;
            }
            switch (values.FirstOrDefault())
            {
                case "rename":
                    await RespondWithModalAsync(modalFactory.RenameModal(voiceChannelId).Build());
                    break;
                case "userLimit":
                    await RespondWithModalAsync(modalFactory.UserLimitModal(voiceChannelId).Build());
                    break;
                case "lock":
                    await SetVoiceChannelAccessibility(voiceChannelId, true);
                    await RespondAsync("The channel has been locked.", ephemeral: true);
                    break;
                case "unlock":
                    await SetVoiceChannelAccessibility(voiceChannelId, false);
                    await RespondAsync("The channel has been unlocked.", ephemeral: true);
                    break;
                case "kick":
                    var options = await GetKickableUsersAsync(voiceChannelId);
                    if (!options.Any())
                    {
                        await RespondAsync("You are alone in the channel.", ephemeral: true);
                        return;
                    }
                    await RespondWithModalAsync(modalFactory.KickModal(voiceChannelId, options).Build());
                    break;
            }
        }

        public async Task SetVoiceChannelAccessibility(ulong voiceChannelId, bool @lock)
        {
            var channel = discordSocketClient.GetChannel(voiceChannelId) as SocketVoiceChannel;
            if (channel is null)
                return;
            var everyoneRole = channel.Guild.EveryoneRole;
            if (@lock)
                await channel.AddPermissionOverwriteAsync(everyoneRole, new OverwritePermissions(connect: PermValue.Deny));
            else
                await channel.AddPermissionOverwriteAsync(everyoneRole, new OverwritePermissions(connect: PermValue.Allow));
        }

        public async Task<List<SelectMenuOptionBuilder>> GetKickableUsersAsync(ulong voiceChannelId)
        {
            if (discordSocketClient.GetChannel(voiceChannelId) is not SocketVoiceChannel socketVoiceChannel)
                return new List<SelectMenuOptionBuilder>();
            var ownerId = await temporaryVoiceChannelService.GetOwnerIdAsync(voiceChannelId);
            return socketVoiceChannel.ConnectedUsers
                .Where(user => user.Id != ownerId)
                .Select(user => new SelectMenuOptionBuilder()
                    .WithLabel(user.DisplayName)
                    .WithValue(user.Id.ToString()))
                .ToList();
        }
    }
}
