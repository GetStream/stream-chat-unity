using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.State
{
    internal static class StatefulModelsExt
    {
        [Pure]
        public static List<string> ToUserIdsListOrNull(this IEnumerable<IStreamUser> users) => users?.Select(x => x.Id).ToList();
    }
}