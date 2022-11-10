using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.SampleProject_StateClient.Views;

namespace StreamChat.SampleProject_StateClient
{
    /// <summary>
    /// Keep chat state
    /// </summary>
    public interface IChatState : IDisposable
    {
        event Action<IStreamChannel> ActiveChanelChanged;
        event Action ChannelsUpdated;
        event Action<IStreamMessage> MessageEditRequested;

        IStreamChannel ActiveChannel { get; }
        IReadOnlyList<IStreamChannel> Channels { get; }
        IStreamChatStateClient Client { get; }

        void OpenChannel(IStreamChannel channel);

        void EditMessage(IStreamMessage message);

        Task<IStreamChannel> CreateNewChannelAsync(string channelName);

        void ShowPopup<TPopup>()
            where TPopup : BaseFullscreenPopup;

        void HidePopup<TPopup>(TPopup instance)
            where TPopup : BaseFullscreenPopup;

        Task UpdateChannelsAsync();

        Task LoadPreviousMessagesAsync();
    }
}