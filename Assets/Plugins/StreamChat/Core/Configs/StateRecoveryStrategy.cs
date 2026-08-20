namespace StreamChat.Core.Configs
{
    /// <summary>
    /// How <see cref="IStreamChatClient"/> restores local state after the websocket reconnects.
    /// Set through <see cref="IStreamClientConfig.StateRecoveryStrategy"/>.
    /// </summary>
    /// <remarks>
    /// Regardless of the strategy, a reconnect always drops the server-side watches that were
    /// established before the disconnect. <see cref="ReplayEvents"/> and <see cref="BatchStateUpdate"/>
    /// re-establish them; <see cref="Disabled"/> leaves that to you.
    /// </remarks>
    public enum StateRecoveryStrategy
    {
        /// <summary>
        /// Default, and the behaviour of every SDK version before this option existed.
        ///
        /// The client calls <c>/sync</c> for the channels it was watching and replays each missed
        /// event through the normal event pipeline, so every per-event callback
        /// (<see cref="StatefulModels.IStreamChannel.MessageReceived"/>,
        /// <see cref="StatefulModels.IStreamChannel.ReactionAdded"/>, and so on) fires exactly as it
        /// would for a live event. It then re-queries and re-watches those channels unconditionally,
        /// which is new: previously a failed or skipped <c>/sync</c> left the channels stale and
        /// unwatched for the rest of the connection.
        ///
        /// Choose this when your UI is driven by per-event callbacks. The cost is that a long outage
        /// on a busy channel replays up to ~1000 events in a single frame, which is visible as a
        /// hitch on mobile.
        /// </summary>
        ReplayEvents = 0,

        /// <summary>
        /// Same recovery pipeline as <see cref="ReplayEvents"/>, but the <c>/sync</c> events are
        /// applied to local state without raising the per-event callbacks whose effect is observable
        /// in model state afterwards. Subscribe to <see cref="IStreamChatClient.StateRecovered"/> and
        /// rebuild from <see cref="StatefulModels.IStreamChannel.Messages"/> and friends instead.
        ///
        /// Callbacks that carry information the SDK cannot reconstruct from state are still raised
        /// per event: <see cref="StatefulModels.IStreamChannel.CustomEventReceived"/>,
        /// <see cref="IStreamChatClient.ChannelDeleted"/>, and the local-user membership and invite
        /// notifications.
        ///
        /// Choose this when a long outage causes a frame hitch on resume. This is the cheapest
        /// recovery the SDK offers.
        /// </summary>
        BatchStateUpdate = 1,

        /// <summary>
        /// The SDK performs no recovery after a reconnect: no <c>/sync</c>, no re-query, no re-watch,
        /// and no <see cref="IStreamChatClient.StateRecovered"/>. Local state is left exactly as it
        /// was and <see cref="IStreamChatClient.WatchedChannels"/> is left untouched, so it remains
        /// the list of what you were watching before the drop.
        ///
        /// Choose this only if you own recovery. Subscribe to
        /// <see cref="IStreamChatClient.ConnectionStateChanged"/>, and on the transition to
        /// <see cref="LowLevelClient.ConnectionState.Connected"/> re-hydrate and re-watch yourself with
        /// <c>QueryChannelsAsync(new[] { ChannelFilter.Cid.In(cids) }, limit: 30)</c> - that single
        /// call both refreshes state and re-establishes the watches. Note that
        /// <see cref="StatefulModels.IStreamChannel.WatchAsync"/> is a no-op for a channel whose
        /// <see cref="StatefulModels.IStreamChannel.IsWatched"/> is still <c>true</c>, which is the
        /// case here, so use the query.
        /// </summary>
        Disabled = 2,
    }
}
