using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.Models;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;
using StreamChat.Core.State;
using StreamChat.Core.State.Caches;

namespace StreamChat.Core.StatefulModels
{
    internal sealed class StreamPoll : StreamStatefulModelBase<StreamPoll>,
        IUpdateableFrom<PollResponseDataInternalDTO, StreamPoll>,
        IStreamPoll
    {
        public event StreamPollHandler Closed;
        public event StreamPollHandler Updated;
        public event StreamPollVoteHandler VoteCasted;
        public event StreamPollVoteHandler VoteChanged;
        public event StreamPollVoteHandler VoteRemoved;

        public bool AllowAnswers { get; private set; }

        public bool AllowUserSuggestedOptions { get; private set; }

        public int AnswersCount { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public IStreamUser CreatedBy { get; private set; }

        public string CreatedById { get; private set; }

        public string Description { get; private set; }

        public bool EnforceUniqueVote { get; private set; }

        public string Id { get; private set; }

        public bool? IsClosed
        {
            get => _isClosed;
            private set
            {
                if (_isClosed == value)
                {
                    return;
                }

                _isClosed = value;

                if (value == true)
                {
                    Closed?.Invoke(this);
                }
            }
        }

        public IReadOnlyList<StreamPollVote> LatestAnswers => _latestAnswers;

        public IReadOnlyDictionary<string, IReadOnlyList<StreamPollVote>> LatestVotesByOption => _latestVotesByOption;

        public int? MaxVotesAllowed { get; private set; }

        public string Name { get; private set; }

        public IReadOnlyList<StreamPollOption> Options => _options;

        public IReadOnlyList<StreamPollVote> OwnVotes => _ownVotes;

        public DateTimeOffset UpdatedAt { get; private set; }

        public int VoteCount { get; private set; }

        public IReadOnlyDictionary<string, int> VoteCountsByOption => _voteCountsByOption;

        public VotingVisibility VotingVisibility { get; private set; }

        public IStreamChannel Channel => _channel;

        public string MessageId { get; private set; }

        public async Task<StreamPollVote> CastVoteAsync(string optionId)
        {
            StreamAsserts.AssertNotNullOrEmpty(optionId, nameof(optionId));

            var request = new CastPollVoteRequestInternalDTO
            {
                Vote = new VoteDataInternalDTO
                {
                    OptionId = optionId
                }
            };

            var response = await LowLevelClient.InternalPollsApi.CastVoteAsync(MessageId, Id, request);
            return new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(response.Vote, Cache);
        }

        public async Task RemoveVoteAsync(string voteId)
        {
            StreamAsserts.AssertNotNullOrEmpty(voteId, nameof(voteId));

            await LowLevelClient.InternalPollsApi.RemoveVoteAsync(MessageId, Id, voteId);
        }

        public async Task UpdateAsync(StreamUpdatePollRequest updateRequest)
        {
            StreamAsserts.AssertNotNull(updateRequest, nameof(updateRequest));

            var requestDto = updateRequest.TrySaveToDto();

            var response = await LowLevelClient.InternalPollsApi.UpdatePollAsync(Id, requestDto);

            // Update from response
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(response.Poll, Cache);
        }

        public async Task CloseAsync()
        {
            var request = new UpdatePollPartialRequestInternalDTO
            {
                Set = new Dictionary<string, object>
                {
                    { "is_closed", true }
                }
            };

            var response = await LowLevelClient.InternalPollsApi.UpdatePollPartialAsync(Id, request);

            // Update from response
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(response.Poll, Cache);
        }

        public async Task<StreamPollOption> AddOptionAsync(string text)
        {
            StreamAsserts.AssertNotNullOrEmpty(text, nameof(text));

            var request = new CreatePollOptionRequestInternalDTO
            {
                Text = text
            };

            var response = await LowLevelClient.InternalPollsApi.CreatePollOptionAsync(Id, request);
            
            
            // Return the option as public model
            return new StreamPollOption().TryLoadFromDto<PollOptionResponseDataInternalDTO, StreamPollOption>(response.PollOption, Cache);
        }

        public async Task<StreamPollOption> UpdateOptionAsync(string optionId, string text)
        {
            StreamAsserts.AssertNotNullOrEmpty(optionId, nameof(optionId));
            StreamAsserts.AssertNotNullOrEmpty(text, nameof(text));

            var request = new UpdatePollOptionRequestInternalDTO
            {
                Text = text
            };

            var response = await LowLevelClient.InternalPollsApi.UpdatePollOptionAsync(Id, optionId, request);
            
            
            // Return the option as public model
            return new StreamPollOption().TryLoadFromDto<PollOptionResponseDataInternalDTO, StreamPollOption>(response.PollOption, Cache);
        }

        public async Task DeleteOptionAsync(string optionId)
        {
            StreamAsserts.AssertNotNullOrEmpty(optionId, nameof(optionId));

            await LowLevelClient.InternalPollsApi.DeletePollOptionAsync(Id, optionId);
        }

        void IUpdateableFrom<PollResponseDataInternalDTO, StreamPoll>.UpdateFromDto(PollResponseDataInternalDTO dto, ICache cache)
        {
            AllowAnswers = dto.AllowAnswers;
            AllowUserSuggestedOptions = dto.AllowUserSuggestedOptions;
            AnswersCount = dto.AnswersCount;
            CreatedAt = dto.CreatedAt;

            if (dto.CreatedBy != null)
            {
                CreatedBy = cache.TryCreateOrUpdate(dto.CreatedBy);
            }

            CreatedById = dto.CreatedById;
            Description = dto.Description;
            EnforceUniqueVote = dto.EnforceUniqueVote;
            Id = dto.Id;
            IsClosed = dto.IsClosed;

            if (dto.LatestAnswers != null)
            {
                _latestAnswers.Clear();
                _latestAnswers.AddRange(dto.LatestAnswers.Select(v => new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(v, cache)));
            }

            if (dto.LatestVotesByOption != null)
            {
                _latestVotesByOption.Clear();
                foreach (var kvp in dto.LatestVotesByOption)
                {
                    _latestVotesByOption[kvp.Key] = kvp.Value.Select(v => new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(v, cache)).ToList();
                }
            }

            MaxVotesAllowed = dto.MaxVotesAllowed;
            Name = dto.Name;

            if (dto.Options != null)
            {
                _options.Clear();
                _options.AddRange(dto.Options.Select(o => new StreamPollOption().TryLoadFromDto<PollOptionResponseDataInternalDTO, StreamPollOption>(o, cache)));
            }

            if (dto.OwnVotes != null)
            {
                _ownVotes.Clear();
                _ownVotes.AddRange(dto.OwnVotes.Select(v => new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(v, cache)));
            }

            UpdatedAt = dto.UpdatedAt;
            VoteCount = dto.VoteCount;

            if (dto.VoteCountsByOption != null)
            {
                _voteCountsByOption.Clear();
                foreach (var kvp in dto.VoteCountsByOption)
                {
                    _voteCountsByOption[kvp.Key] = kvp.Value;
                }
            }

            VotingVisibility = dto.VotingVisibility;

            LoadAdditionalProperties(dto.AdditionalProperties);

            // Notify subscribers that poll was updated
            Updated?.Invoke(this);
        }

        internal void HandlePollClosedEvent(PollClosedEventInternalDTO dto)
        {
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(dto.Poll, Cache);
        }

        internal void HandlePollUpdatedEvent(PollUpdatedEventInternalDTO dto)
        {
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(dto.Poll, Cache);
        }

        internal void HandlePollVoteCastedEvent(PollVoteCastedEventInternalDTO dto)
        {
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(dto.Poll, Cache);

            if (dto.PollVote != null)
            {
                var vote = new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(dto.PollVote, Cache);
                VoteCasted?.Invoke(this, vote);
            }
        }

        internal void HandlePollVoteChangedEvent(PollVoteChangedEventInternalDTO dto)
        {
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(dto.Poll, Cache);

            if (dto.PollVote != null)
            {
                var vote = new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(dto.PollVote, Cache);
                VoteChanged?.Invoke(this, vote);
            }
        }

        internal void HandlePollVoteRemovedEvent(PollVoteRemovedEventInternalDTO dto)
        {
            this.TryUpdateFromDto<PollResponseDataInternalDTO, StreamPoll>(dto.Poll, Cache);

            if (dto.PollVote != null)
            {
                var vote = new StreamPollVote().TryLoadFromDto<PollVoteResponseDataInternalDTO, StreamPollVote>(dto.PollVote, Cache);
                VoteRemoved?.Invoke(this, vote);
            }
        }

        internal void InternalSetChannel(IStreamChannel channel) => _channel = channel;

        internal void InternalSetMessageId(string messageId) => MessageId = messageId;

        protected override string InternalUniqueId
        {
            get => Id;
            set => Id = value;
        }

        protected override StreamPoll Self => this;

        internal StreamPoll(string uniqueId, ICacheRepository<StreamPoll> repository,
            IStatefulModelContext context)
            : base(uniqueId, repository, context)
        {
        }

        private bool? _isClosed;
        private IStreamChannel _channel;
        private readonly List<StreamPollVote> _latestAnswers = new List<StreamPollVote>();
        private readonly Dictionary<string, IReadOnlyList<StreamPollVote>> _latestVotesByOption = new Dictionary<string, IReadOnlyList<StreamPollVote>>();
        private readonly List<StreamPollOption> _options = new List<StreamPollOption>();
        private readonly List<StreamPollVote> _ownVotes = new List<StreamPollVote>();
        private readonly Dictionary<string, int> _voteCountsByOption = new Dictionary<string, int>();
        private readonly Dictionary<string, object> _additionalProperties = new Dictionary<string, object>();
    }
}


