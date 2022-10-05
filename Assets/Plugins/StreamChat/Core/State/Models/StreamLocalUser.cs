using StreamChat.Core.Models;
using StreamChat.Core.State;

namespace StreamChat.Core.State.Models
{
    public class StreamLocalUser : StreamUser
    {
        // public StreamLocalUser()
        // {
        //
        // }
        //
        // internal StreamLocalUser(OwnUser ownUser)
        // {
        //     UpdateFrom(ownUser);
        // }

        public void UpdateFrom(OwnUser ownUser)
        {
        }

        internal static StreamLocalUser Create(string uniqueId, IRepository<StreamLocalUser> repository)
            => new StreamLocalUser(uniqueId, repository);

        internal StreamLocalUser(string uniqueId, IRepository<StreamLocalUser> repository)
            : base(uniqueId, repository as IRepository<StreamUser>)
        {
        }
    }
}