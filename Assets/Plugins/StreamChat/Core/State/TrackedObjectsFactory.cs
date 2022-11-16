﻿using System;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.State.TrackedObjects;
using StreamChat.Core.State.Caches;
using StreamChat.Core;
using StreamChat.Libs.Logs;

namespace StreamChat.Core.State
{
    /// <summary>
    /// Factory for <see cref="IStreamTrackedObject"/>
    /// </summary>
    internal sealed class TrackedObjectsFactory : ITrackedObjectsFactory
    {
        public TrackedObjectsFactory(StreamChatClient streamChatClient, ILogs logs, Cache cache)
        {
            _streamChatClient = streamChatClient ?? throw new ArgumentNullException(nameof(streamChatClient));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            _context = new TrackedObjectContext(_cache, streamChatClient, logs);
        }

        public StreamChannel CreateStreamChannel(string uniqueId)
            => new StreamChannel(uniqueId, _cache.Channels, _context);

        public StreamChannelMember CreateStreamChannelMember(string uniqueId)
            => new StreamChannelMember(uniqueId, _cache.ChannelMembers, _context);

        public StreamLocalUserData CreateStreamLocalUser(string uniqueId)
            => new StreamLocalUserData(uniqueId, _cache.LocalUser, _context);

        public StreamMessage CreateStreamMessage(string uniqueId)
            => new StreamMessage(uniqueId, _cache.Messages, _context);

        public StreamUser CreateStreamUser(string uniqueId)
            => new StreamUser(uniqueId, _cache.Users, _context);

        private readonly ILogs _logs;
        private readonly StreamChatClient _streamChatClient;
        private readonly ITrackedObjectContext _context;
        private readonly Cache _cache;
    }
}