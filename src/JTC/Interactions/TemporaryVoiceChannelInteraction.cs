namespace JTC.Interactions
{
    /// <summary>
    /// Handles interactions related to temporary voice channels.
    /// </summary>
    /// <param name="discordSocketClient">The Discord client used to access voice channels.</param>
    /// <param name="modalFactory">Factory for creating modals.</param>
    /// <param name="temporaryVoiceChannelService">Service for managing temporary voice channels.</param>
    public class TemporaryVoiceChannelInteraction(
        DiscordSocketClient discordSocketClient, 
        ModalFactory modalFactory, 
        TemporaryVoiceChannelService temporaryVoiceChannelService) : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// Handles settings interactions for a temporary voice channel.
        /// Routes the interaction to the appropriate handler based on the selected value.
        /// </summary>
        /// <param name="rawData">The raw custom ID of the interaction.</param>
        /// <param name="values">The selected values from the interaction menu.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Locks or unlocks a temporary voice channel by modifying permissions for the @everyone role.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="lock">True to lock, false to unlock.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves the list of users that can be kicked from a temporary voice channel, excluding the channel owner.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <returns>A list of <see cref="SelectMenuOptionBuilder"/> representing kickable users.</returns>
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
