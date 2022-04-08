using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Models;
using StreamChat.Core.Requests;

namespace Plugins.StreamChat.Samples.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/query_users/?language=unity
    /// </summary>
    public class UsersCodeSamples
    {
        private async Task QueryUsers()
        {
            var response = await Client.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "id", new Dictionary<string, object>
                        {
                            {
                                "$in", new List<string>
                                {
                                    "new-user-3", "new-user-4"
                                }
                            }
                        }
                    }
                },
            });
        }

        private async Task QueryUsersExtended()
        {
            var response = await Client.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                Limit = 30,
                Offset = 0,

                Sort = new List<SortParam>
                {
                    new SortParam
                    {
                        Field = "created_at",
                        Direction = -1,
                    }
                },

                //Request user presence status
                Presence = true,
            });
        }

        private async Task QueryUsersBanned()
        {
            var response = await Client.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    {
                        "banned", new Dictionary<string, object>
                        {
                            {
                                "$eq", true
                            }
                        }
                    }
                },
            });
        }

        // private async Task SwitchUser()
        // {
        //     //Dispose previous client
        //     //Create new client
        // }


        private IStreamChatClient Client;
    }
}