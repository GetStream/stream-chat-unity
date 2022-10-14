using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using StreamChat.Core.State.TrackedObjects;

namespace StreamChat.Core.State
{
    public static class TrackedObjectsExt
    {
        [Pure]
        public static List<string> ToUserIdsListOrNull(this IEnumerable<StreamUser> users) => users?.Select(x => x.Id).ToList();
    }
}