using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.SampleProject.Views;

namespace StreamChat.SampleProject
{
    /// <summary>
    /// Keep chat state
    /// </summary>
    public interface IChatState : IDisposable
    {
        event Action<ChannelState> ActiveChanelChanged;
        event Action<ChannelState, Message> ActiveChanelMessageReceived;
        event Action ChannelsUpdated;
        event Action<Message> MessageEditRequested;

        ChannelState ActiveChannel { get; }
        IReadOnlyList<ChannelState> Channels { get; }
        IStreamChatClient Client { get; }

        void OpenChannel(ChannelState channel);

        void EditMessage(Message message);

        Task<ChannelState> CreateNewChannelAsync(string channelName);

        void ShowPopup<TPopup>()
            where TPopup : BaseFullscreenPopup;

        void HidePopup<TPopup>(TPopup instance)
            where TPopup : BaseFullscreenPopup;

        Task UpdateChannelsAsync();

        Task LoadPreviousMessagesAsync();

        void MarkMessageAsLastRead(Message message);
    }
}