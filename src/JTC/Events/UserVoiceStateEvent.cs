namespace JTC.Events
{
    public class UserVoiceStateEvent(ComponentFactory componentFactory, HubService hubService, TemporaryVoiceChannelService temporaryVoiceChannelService)
    {
        public async Task OnUserVoiceStateUpdatedAsync(SocketUser socketUser, SocketVoiceState before, SocketVoiceState after)
        {
            if (before.VoiceChannel?.Id == after.VoiceChannel?.Id)
                return;
            if (before.VoiceChannel is SocketVoiceChannel leftChannel && await temporaryVoiceChannelService.IsTemporaryVoiceChannelAsync(leftChannel.Id))
                await UserLeftTemporaryVoiceChannelAsync(leftChannel);
            if (after.VoiceChannel is SocketVoiceChannel joinedChannel && socketUser is SocketGuildUser socketGuildUser)
            {
                var hub = await hubService.GetHubAsync(joinedChannel.Id);
                if (hub is not null)
                    await UserJoinHubAsync(socketGuildUser, joinedChannel, hub);
            }
        }

        private async Task UserLeftTemporaryVoiceChannelAsync(SocketVoiceChannel socketVoiceChannel)
        {
            if (socketVoiceChannel.ConnectedUsers.Any())
                return;
            await socketVoiceChannel.DeleteAsync();
            await temporaryVoiceChannelService.RemoveTemporaryVoiceChannelAsync(socketVoiceChannel.Id);
        }

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
