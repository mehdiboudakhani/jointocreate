namespace JTC.Events
{
    /// <summary>
    /// Handles events related to user joining or leaving voice channels.
    /// </summary>
    /// <param name="componentFactory">Factory for creating Discord component interface.</param>
    /// <param name="hubService">Service for managing hubs.</param>
    /// <param name="temporaryVoiceChannelService">Service for managing temporary voice channels.</param>
    public class UserVoiceStateEvent(ComponentFactory componentFactory, HubService hubService, TemporaryVoiceChannelService temporaryVoiceChannelService)
    {
        /// <summary>
        /// Called when a user's voice state changes.
        /// Routes to the appropriate handler.
        /// </summary>
        /// <param name="socketUser">The user whose voice state changed.</param>
        /// <param name="before">The previous voice state.</param>
        /// <param name="after">The new voice state.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnUserVoiceStateUpdatedAsync(SocketUser socketUser, SocketVoiceState before, SocketVoiceState after)
        {
            // User didn't change channels
            if (before.VoiceChannel?.Id == after.VoiceChannel?.Id)
                return;

            // Handle a user leaving a temporary voice channel
            if (before.VoiceChannel is SocketVoiceChannel leftChannel && await temporaryVoiceChannelService.IsTemporaryVoiceChannelAsync(leftChannel.Id))
                await UserLeftTemporaryVoiceChannelAsync(leftChannel);

            // Handle a user joining a hub
            if (after.VoiceChannel is SocketVoiceChannel joinedChannel && socketUser is SocketGuildUser socketGuildUser)
            {
                var hub = await hubService.GetHubAsync(joinedChannel.Id);
                if (hub is not null)
                    await UserJoinHubAsync(socketGuildUser, joinedChannel, hub);
            }
        }

        /// <summary>
        /// Handles the logic when a user leaves a temporary voice channel.
        /// Deletes the channel if it is empty and removes it from the database.
        /// </summary>
        /// <param name="socketVoiceChannel">The temporary voice channel that was left.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UserLeftTemporaryVoiceChannelAsync(SocketVoiceChannel socketVoiceChannel)
        {
            if (socketVoiceChannel.ConnectedUsers.Any())
                return;
            await socketVoiceChannel.DeleteAsync();
            await temporaryVoiceChannelService.RemoveTemporaryVoiceChannelAsync(socketVoiceChannel.Id);
        }

        /// <summary>
        /// Handles the logic when a user joins a hub voice channel.
        /// Creates a new temporary voice channel.
        /// </summary>
        /// <param name="socketGuildUser">The user joining the hub.</param>
        /// <param name="socketVoiceChannel">The hub channel being joined.</param>
        /// <param name="hub">The hub entity containing configuration for the temporary voice channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UserJoinHubAsync(SocketGuildUser socketGuildUser, SocketVoiceChannel socketVoiceChannel, HubEntity hub)
        {
            var channel = await socketVoiceChannel.Guild.CreateVoiceChannelAsync(hub.ChildName.Replace("{user}", socketGuildUser.DisplayName), props =>
            {
                props.CategoryId = socketVoiceChannel.CategoryId;
                props.UserLimit = hub.UserLimit;
            });
            await socketGuildUser.ModifyAsync(user => user.Channel = channel);
            await temporaryVoiceChannelService.AddTemporaryVoiceChannelAsync(channel.Id, hub.VoiceChannelId, socketVoiceChannel.Guild.Id, socketGuildUser.Id);
            await channel.AddPermissionOverwriteAsync(socketGuildUser, new OverwritePermissions(connect: PermValue.Allow));
            await channel.AddPermissionOverwriteAsync(socketVoiceChannel.Guild.CurrentUser, new OverwritePermissions(connect: PermValue.Allow));
            if (hub.TemporaryVoiceChannelInterface)
                await channel.SendMessageAsync(components: componentFactory.TemporaryVoiceChannelInterface(channel.Id, socketGuildUser.Id).Build());
        }
    }
}
