using StreamChat.Core.InternalDTO.Requests;

namespace StreamChat.Core.LowLevelClient.Models
{
    /// <summary>
    /// Voting visibility for polls
    /// </summary>
    public readonly struct VotingVisibility : System.IEquatable<VotingVisibility>
    {
        /// <summary>
        /// Anonymous voting - votes are not associated with users
        /// </summary>
        public static readonly VotingVisibility Anonymous = new VotingVisibility("anonymous");
        
        /// <summary>
        /// Public voting - votes are visible to all users
        /// </summary>
        public static readonly VotingVisibility Public = new VotingVisibility("public");

        public VotingVisibility(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;

        public bool Equals(VotingVisibility other) => _value == other._value;

        public override bool Equals(object obj) => obj is VotingVisibility other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(VotingVisibility left, VotingVisibility right) => left.Equals(right);

        public static bool operator !=(VotingVisibility left, VotingVisibility right) => !left.Equals(right);

        public static implicit operator VotingVisibility(string value) => new VotingVisibility(value);

        public static implicit operator string(VotingVisibility type) => type._value;

        internal CreatePollRequestVotingVisibilityInternalDTO ToCreatePollRequestDto()
            => new CreatePollRequestVotingVisibilityInternalDTO(_value);

        internal UpdatePollRequestVotingVisibilityInternalDTO ToUpdatePollRequestDto()
            => new UpdatePollRequestVotingVisibilityInternalDTO(_value);

        private readonly string _value;
    }
}

