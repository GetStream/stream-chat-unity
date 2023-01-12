using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Core.LowLevelClient;
using StreamChat.Core.LowLevelClient.Requests;
using UnityEngine;

namespace StreamChat.Samples.LowLevelClient.ClientDocs
{
    /// <summary>
    /// Code samples for Channels sections: https://getstream.io/chat/docs/unity/query_users/?language=unity
    /// </summary>
    public class UsersCodeSamples
    {
        private async Task QueryUsers()
        {
            var response = await _lowLevelClient.UserApi.QueryUsersAsync(new QueryUsersRequest
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
            var response = await _lowLevelClient.UserApi.QueryUsersAsync(new QueryUsersRequest
            {
                Limit = 30,
                Offset = 0,

                Sort = new List<SortParamRequest>
                {
                    new SortParamRequest
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
            var response = await _lowLevelClient.UserApi.QueryUsersAsync(new QueryUsersRequest
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

        private async Task UpsertUsers()
        {
            var createNewUserRequest = new UserObjectRequest
            {
                Id = "my-new-user-id-555",
                Role = "user",
                AdditionalProperties = new Dictionary<string, object>()
                {
                    { "Name", "David" },
                    { "Age", 24 },
                    { "Passions", new string[]{"Tennis", "Football", "Basketball"}}
                }
            };

            try
            {
                var updateUsersResponse = await _lowLevelClient.UserApi.UpsertManyUsersAsync(new UpdateUsersRequest
                {
                    Users = new Dictionary<string, UserObjectRequest>
                    {
                        {createNewUserRequest.Id, createNewUserRequest}
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private IStreamChatLowLevelClient _lowLevelClient;
    }
}