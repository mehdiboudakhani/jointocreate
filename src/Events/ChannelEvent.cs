namespace JTC.Events
{
    /// <summary>
    /// Handles events related to Discord channels.
    /// </summary>
    /// <param name="hubService">Service for managing hubs.</param>
    public class ChannelEvent(HubService hubService)
    {
        /// <summary>
        /// Called when a Discord channel is destroyed.
        /// Removes the channel from the database if it was a hub.
        /// </summary>
        /// <param name="socketChannel">The channel that was destroyed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnChannelDestroyedAsync(SocketChannel socketChannel)
        {
            if (socketChannel is not SocketVoiceChannel socketVoiceChannel)
                return;
            if (await hubService.IsHubAsync(socketVoiceChannel.Id))
                await hubService.RemoveHubAsync(socketVoiceChannel.Id);
        }
    }
}
