namespace JTC.Events
{
    public class ChannelEvent(HubService hubService)
    {
        public async Task OnChannelDestroyedAsync(SocketChannel socketChannel)
        {
            if (socketChannel is not SocketVoiceChannel socketVoiceChannel)
                return;
            if (await hubService.IsHubAsync(socketVoiceChannel.Id))
                await hubService.RemoveHubAsync(socketVoiceChannel.Id);
        }
    }
}
