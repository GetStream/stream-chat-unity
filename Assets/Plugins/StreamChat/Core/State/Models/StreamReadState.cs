namespace StreamChat.Core.State.Models
{
    public class StreamReadState
    {
        public System.DateTimeOffset? LastRead { get; set; }

        public int? UnreadMessages { get; set; }

        public StreamUser User { get; set; }
    }
}