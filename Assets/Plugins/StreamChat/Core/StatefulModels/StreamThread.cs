using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Models;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.StatefulModels
{
    internal sealed class StreamThread : StreamStatefulModelBase<StreamThread>,
        IUpdateableFrom<ThreadStateResponseInternalDTO, StreamThread>,
        IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>,
        IStreamThread
    {
        public event StreamThreadChangeHandler Updated;
        public event StreamThreadReplyHandler ReplyReceived;
        public event StreamThreadReadHandler ReadStateChanged;

        public int? ActiveParticipantCount { get; private set; }

        public IStreamChannel Channel { get; private set; }

        public string ChannelCid { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public IStreamUser CreatedBy { get; private set; }

        public string CreatedByUserId { get; private set; }

        public IReadOnlyDictionary<string, object> CustomData => _custom;

        public DateTimeOffset? DeletedAt { get; private set; }

        public DateTimeOffset? LastMessageAt { get; private set; }

        public IReadOnlyList<IStreamMessage> LatestReplies => _latestReplies;

        public IStreamMessage ParentMessage { get; private set; }

        public string ParentMessageId { get; private set; }

        public int? ParticipantCount { get; private set; }

        public IReadOnlyList<StreamRead> Read => _read;

        public int? ReplyCount { get; private set; }

        public IReadOnlyList<StreamThreadParticipant> ThreadParticipants => _threadParticipants;

        public string Title { get; private set; }

        public DateTimeOffset UpdatedAt { get; private set; }

        public async Task RefreshAsync(int? replyLimit = null, int? participantLimit = null)
        {
            var response = await LowLevelClient.InternalThreadsApi.GetThreadAsync(ParentMessageId,
                replyLimit: replyLimit, participantLimit: participantLimit, watch: true);
            ((IUpdateableFrom<ThreadStateResponseInternalDTO, StreamThread>)this).UpdateFromDto(response.Thread, Cache);
        }

        public async Task<IReadOnlyList<IStreamMessage>> LoadOlderRepliesAsync(int limit = 25)
        {
            StreamAsserts.AssertGreaterThanZero(limit, nameof(limit));

            var pagination = new MessagePaginationParamsRequestInternalDTO
            {
                Limit = limit,
            };

            var oldest = _latestReplies.FirstOrDefault();
            if (oldest != null)
            {
                pagination.IdLt = oldest.Id;
            }

            var response = await LowLevelClient.InternalThreadsApi.GetRepliesAsync(ParentMessageId, pagination);

            var loaded = new List<IStreamMessage>();
            if (response.Messages != null)
            {
                foreach (var dto in response.Messages)
                {
                    var message = Cache.TryCreateOrUpdate(dto);
                    if (message != null)
                    {
                        loaded.Add(message);
                    }
                }
            }

            MergeIntoLatestReplies(loaded);

            return loaded;
        }

        public async Task UpdatePartialAsync(IDictionary<string, object> setFields = null,
            IEnumerable<string> unsetFields = null)
        {
            if (setFields == null && unsetFields == null)
            {
                throw new ArgumentException($"{nameof(setFields)} and {nameof(unsetFields)} cannot be both null");
            }

            var request = new UpdateThreadPartialRequestInternalDTO
            {
                Set = setFields?.ToDictionary(p => p.Key, p => p.Value),
                Unset = unsetFields?.ToList(),
            };

            var response = await LowLevelClient.InternalThreadsApi.UpdateThreadPartialAsync(ParentMessageId, request);

            // Server returns the updated ThreadResponse - apply it
            if (response.Thread != null)
            {
                ((IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>)this).UpdateFromDto(response.Thread, Cache);
            }
        }

        public Task MarkReadAsync()
        {
            if (Channel == null)
            {
                throw new InvalidOperationException(
                    $"Cannot mark thread {ParentMessageId} as read because its parent channel is not loaded.");
            }

            return Channel.MarkThreadAsReadAsync(ParentMessageId);
        }

        public Task MarkUnreadAsync()
        {
            if (Channel == null)
            {
                throw new InvalidOperationException(
                    $"Cannot mark thread {ParentMessageId} as unread because its parent channel is not loaded.");
            }

            return Channel.MarkThreadAsUnreadAsync(ParentMessageId);
        }

        void IUpdateableFrom<ThreadStateResponseInternalDTO, StreamThread>.UpdateFromDto(
            ThreadStateResponseInternalDTO dto, ICache cache)
        {
            ActiveParticipantCount = GetOrDefault(dto.ActiveParticipantCount, ActiveParticipantCount);

            if (dto.Channel != null)
            {
                Channel = cache.TryCreateOrUpdate(dto.Channel);
            }

            ChannelCid = GetOrDefault(dto.ChannelCid, ChannelCid);
            CreatedAt = dto.CreatedAt;

            if (dto.CreatedBy != null)
            {
                CreatedBy = cache.TryCreateOrUpdate(dto.CreatedBy);
            }

            CreatedByUserId = GetOrDefault(dto.CreatedByUserId, CreatedByUserId);
            DeletedAt = dto.DeletedAt;
            LastMessageAt = dto.LastMessageAt;

            if (dto.LatestReplies != null)
            {
                _latestReplies.TryReplaceTrackedObjects2(dto.LatestReplies, cache.Messages);
                SortLatestRepliesByCreatedAt();
            }

            if (dto.ParentMessage != null)
            {
                ParentMessage = cache.TryCreateOrUpdate(dto.ParentMessage);
            }

            ParentMessageId = GetOrDefault(dto.ParentMessageId, ParentMessageId);
            ParticipantCount = GetOrDefault(dto.ParticipantCount, ParticipantCount);
            ReplyCount = dto.ReplyCount;

            _read.TryReplaceRegularObjectsFromDto2(dto.Read, cache);

            if (dto.ThreadParticipants != null)
            {
                _threadParticipants.TryReplaceRegularObjectsFromDto(dto.ThreadParticipants, cache);
            }

            Title = GetOrDefault(dto.Title, Title);
            UpdatedAt = dto.UpdatedAt;

            LoadAdditionalCustom(dto.Custom);

            Updated?.Invoke(this);
        }

        void IUpdateableFrom2<ThreadResponseInternalDTO, StreamThread>.UpdateFromDto(
            ThreadResponseInternalDTO dto, ICache cache)
        {
            ActiveParticipantCount = GetOrDefault(dto.ActiveParticipantCount, ActiveParticipantCount);

            if (dto.Channel != null)
            {
                Channel = cache.TryCreateOrUpdate(dto.Channel);
            }

            ChannelCid = GetOrDefault(dto.ChannelCid, ChannelCid);
            CreatedAt = dto.CreatedAt;

            if (dto.CreatedBy != null)
            {
                CreatedBy = cache.TryCreateOrUpdate(dto.CreatedBy);
            }

            CreatedByUserId = GetOrDefault(dto.CreatedByUserId, CreatedByUserId);
            DeletedAt = dto.DeletedAt;
            LastMessageAt = dto.LastMessageAt;

            if (dto.ParentMessage != null)
            {
                ParentMessage = cache.TryCreateOrUpdate(dto.ParentMessage);
            }

            ParentMessageId = GetOrDefault(dto.ParentMessageId, ParentMessageId);
            ParticipantCount = GetOrDefault(dto.ParticipantCount, ParticipantCount);
            ReplyCount = dto.ReplyCount;

            if (dto.ThreadParticipants != null)
            {
                _threadParticipants.TryReplaceRegularObjectsFromDto(dto.ThreadParticipants, cache);
            }

            Title = GetOrDefault(dto.Title, Title);
            UpdatedAt = dto.UpdatedAt;

            LoadAdditionalCustom(dto.Custom);

            Updated?.Invoke(this);
        }

        internal void HandleNewReply(IStreamMessage reply)
        {
            if (reply == null)
            {
                return;
            }

            var streamReply = (StreamMessage)reply;
            if (!_latestReplies.Contains(streamReply))
            {
                var lastReply = _latestReplies.Count > 0 ? _latestReplies[_latestReplies.Count - 1] : null;
                _latestReplies.Add(streamReply);

                // Local sends or out-of-order WS arrivals can land a reply with an older CreatedAt
                // than the current tail; restore ascending order in those cases.
                if (lastReply != null && streamReply.CreatedAt < lastReply.CreatedAt)
                {
                    SortLatestRepliesByCreatedAt();
                }
            }

            if (ReplyCount.HasValue)
            {
                ReplyCount = ReplyCount.Value + 1;
            }
            else
            {
                ReplyCount = 1;
            }

            LastMessageAt = reply.CreatedAt;

            ReplyReceived?.Invoke(this, reply);
        }

        internal void MergeIntoLatestReplies(IReadOnlyList<IStreamMessage> messages)
        {
            if (messages == null || messages.Count == 0)
            {
                return;
            }

            var inserted = false;
            foreach (var msg in messages)
            {
                var streamMessage = (StreamMessage)msg;
                if (!_latestReplies.Contains(streamMessage))
                {
                    _latestReplies.Add(streamMessage);
                    inserted = true;
                }
            }

            if (inserted)
            {
                SortLatestRepliesByCreatedAt();
            }
        }

        internal void SortLatestRepliesByCreatedAt() => _latestReplies.Sort(MessageCreatedAtComparer.Instance);

        internal void HandleNotifyReadStateChanged()
        {
            ReadStateChanged?.Invoke(this);
        }

        protected override string InternalUniqueId
        {
            get => ParentMessageId;
            set => ParentMessageId = value;
        }

        protected override StreamThread Self => this;

        internal StreamThread(string uniqueId, ICacheRepository<StreamThread> repository,
            IStatefulModelContext context)
            : base(uniqueId, repository, context)
        {
        }

        private void LoadAdditionalCustom(Dictionary<string, object> custom)
        {
            _custom.Clear();
            if (custom == null)
            {
                return;
            }

            foreach (var keyValuePair in custom)
            {
                _custom[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        private readonly Dictionary<string, object> _custom = new Dictionary<string, object>();
        private readonly List<StreamMessage> _latestReplies = new List<StreamMessage>();
        private readonly List<StreamRead> _read = new List<StreamRead>();
        private readonly List<StreamThreadParticipant> _threadParticipants = new List<StreamThreadParticipant>();
    }
}
