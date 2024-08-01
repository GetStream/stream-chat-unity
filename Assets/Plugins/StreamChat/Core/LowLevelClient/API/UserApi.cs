using System;
using System.Threading.Tasks;
using StreamChat.Core.Helpers;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.LowLevelClient.API.Internal;
using StreamChat.Core.LowLevelClient.Requests;
using StreamChat.Core.LowLevelClient.Responses;

namespace StreamChat.Core.LowLevelClient.API
{
    internal class UserApi : IUserApi
    {
        public UserApi(IInternalUserApi internalUserApi)
        {
            _internalUserApi = internalUserApi ?? throw new ArgumentNullException(nameof(internalUserApi));
        }

        public async Task<UsersResponse> QueryUsersAsync(QueryUsersRequest queryUsersRequest)
        {
            var dto = await _internalUserApi.QueryUsersAsync(queryUsersRequest.TrySaveToDto());
            return dto.ToDomain<UsersResponseInternalDTO, UsersResponse>();
        }

        public async Task<GuestResponse> CreateGuestAsync(GuestRequest createGuestRequest)
        {
            var dto = await _internalUserApi.CreateGuestAsync(createGuestRequest.TrySaveToDto());
            return dto.ToDomain<GuestResponseInternalDTO, GuestResponse>();
        }

        public Task<UpdateUsersResponse> UpsertUsersAsync(UpdateUsersRequest updateUsersRequest)
            => UpsertManyUsersAsync(updateUsersRequest);

        public async Task<UpdateUsersResponse> UpsertManyUsersAsync(UpdateUsersRequest updateUsersRequest)
        {
            var dto = await _internalUserApi.UpsertManyUsersAsync(updateUsersRequest.TrySaveToDto());
            return dto.ToDomain<UpdateUsersResponseInternalDTO, UpdateUsersResponse>();
        }

        public async Task<UpdateUsersResponse> UpdateUserPartialAsync(UpdateUserPartialRequest updateUserPartialRequest)
        {
            var dto = await _internalUserApi.UpdateUserPartialAsync(updateUserPartialRequest.TrySaveToDto());
            return dto.ToDomain<UpdateUsersResponseInternalDTO, UpdateUsersResponse>();
        }

        private readonly IInternalUserApi _internalUserApi;
    }
    
    
    public partial class WrappedUnreadCountsResponseInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsChannelTypeInternalDTO> ChannelType { get; set; } = new System.Collections.Generic.List<UnreadCountsChannelTypeInternalDTO>();

        [Newtonsoft.Json.JsonProperty("channels", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsChannelInternalDTO> Channels { get; set; } = new System.Collections.Generic.List<UnreadCountsChannelInternalDTO>();

        /// <summary>
        /// Duration of the request in milliseconds
        /// </summary>
        [Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Duration { get; set; }

        [Newtonsoft.Json.JsonProperty("threads", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.List<UnreadCountsThreadInternalDTO> Threads { get; set; } = new System.Collections.Generic.List<UnreadCountsThreadInternalDTO>();

        [Newtonsoft.Json.JsonProperty("total_unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int TotalUnreadCount { get; set; }

        [Newtonsoft.Json.JsonProperty("total_unread_threads_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int TotalUnreadThreadsCount { get; set; }


    }
        
    public partial class UnreadCountsChannelTypeInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ChannelCount { get; set; }

        [Newtonsoft.Json.JsonProperty("channel_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ChannelType { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }


    }
    
    public partial class UnreadCountsChannelInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("channel_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ChannelId { get; set; }

        [Newtonsoft.Json.JsonProperty("last_read", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastRead { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }


    }
    
    public partial class UnreadCountsThreadInternalDTO
    {
        [Newtonsoft.Json.JsonProperty("last_read", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastRead { get; set; }

        [Newtonsoft.Json.JsonProperty("last_read_message_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LastReadMessageId { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_message_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ParentMessageId { get; set; }

        [Newtonsoft.Json.JsonProperty("unread_count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UnreadCount { get; set; }


    }
}