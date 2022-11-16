using StreamChat.Core;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Events;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    /// <summary>
    /// Code samples related to <see cref="https://getstream.io/chat/docs/unity/event_object/?language=unity"/>
    /// </summary>
    public class EventsClientCodeSamples
    {
        private IStreamChatClient Client;

        public void SimpleSubscribeToEvents()
        {
            Client.MessageReceived += newMessageEvent =>
            {
                var channelId = newMessageEvent.ChannelId;
                var channelType = newMessageEvent.ChannelType;
                var channelCid = newMessageEvent.Cid;
                var messageData = newMessageEvent.Message;
            };

            Client.MessageUpdated += messageUpdatedEvent =>
            {
                var channelId = messageUpdatedEvent.ChannelId;
                var channelType = messageUpdatedEvent.ChannelType;
                var channelCid = messageUpdatedEvent.Cid;
                var messageData = messageUpdatedEvent.Message;
            };

            Client.MessageDeleted += messageDeletedEvent =>
            {
                var channelId = messageDeletedEvent.ChannelId;
                var channelType = messageDeletedEvent.ChannelType;
                var channelCid = messageDeletedEvent.Cid;
                var messageData = messageDeletedEvent.Message;
            };

            Client.EventReceived += serializedEventData => { };

            Client.Connected += (localUser) =>
            {
                //client connected
            };

            Client.ConnectionStateChanged += (prevState, currentState) =>
            {
                //connection changed from prevState to currentState
            };
        }

        private void SubscribeToEvents()
        {
            Client.MessageReceived += OnClientMessageReceived;
            Client.MessageUpdated += OnClientMessageUpdated;
            Client.MessageDeleted += OnClientMessageDeleted;
        }

        private void UnsubscribeFromEvents()
        {
            Client.MessageReceived -= OnClientMessageReceived;
            Client.MessageUpdated -= OnClientMessageUpdated;
            Client.MessageDeleted -= OnClientMessageDeleted;
        }

        private void OnClientMessageReceived(EventMessageNew newMessageEvent)
        {
        }

        private void OnClientMessageDeleted(EventMessageDeleted messageDeletedEvent)
        {
        }

        private void OnClientMessageUpdated(EventMessageUpdated messageDeletedEvent)
        {
        }
    }
}