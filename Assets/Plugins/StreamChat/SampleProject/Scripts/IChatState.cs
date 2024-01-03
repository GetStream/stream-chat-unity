using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.StatefulModels;
using StreamChat.SampleProject.Popups;
using StreamChat.SampleProject.Views;

namespace StreamChat.SampleProject
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
        IStreamChatClient Client { get; }

        void OpenChannel(IStreamChannel channel);

        void EditMessage(IStreamMessage message);

        Task<IStreamChannel> CreateNewChannelAsync(string channelName);

        TPopup ShowPopup<TPopup>()
            where TPopup : BaseFullscreenPopup;

        void HidePopup<TPopup>(TPopup instance)
            where TPopup : BaseFullscreenPopup;

        Task UpdateChannelsAsync();

        Task LoadPreviousMessagesAsync();
    }
}