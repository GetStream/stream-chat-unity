using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Models;
using StreamChat.Core.DTO.Requests;
using StreamChat.Core.DTO.Responses;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Requests;
using StreamChat.Core.Responses;
using StreamChat.Core.Web;

namespace StreamChat.Core.API
{
    /// <summary>
    /// Base Api client
    /// </summary>
    internal abstract class ApiClientBase
    {
        protected ApiClientBase(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _requestUriFactory = requestUriFactory ?? throw new ArgumentNullException(nameof(requestUriFactory));
        }

        protected async Task<TResponseDto> Get<TPayloadDTO, TResponseDto>(string endpoint, TPayloadDTO payload)
        {
            var payloadContent = _serializer.Serialize(payload);
            var parameters = QueryParameters.Default.Append("payload", payloadContent);

            var uri = _requestUriFactory.CreateEndpointUri(endpoint, parameters);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: false);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: true);

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            return responseDto;
        }


        [Obsolete] //Todo: DELETE
        protected async Task<TResponse> Post<TRequest, TRequestDto, TResponse, TResponseDto>(string endpoint,
            TRequest request)
            where TRequest : ISavableTo<TRequestDto>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var result = await Post<TRequestDto, TResponseDto>(endpoint, request.SaveToDto());

            var response = new TResponse();
            response.LoadFromDto(result);

            return response;
        }

        protected async Task<TResponseDto> Post<TRequestDto, TResponseDto>(string endpoint, TRequestDto request)
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint);
            var requestContent = _serializer.Serialize(request);

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await _httpClient.PostAsync(uri, requestContent);
            }
            catch (Exception e)
            {
                _logs.Exception(e);
                throw;
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: false, requestContent);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            TResponseDto responseDto;

            try
            {
                responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            }
            catch (Exception e)
            {
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: false, requestContent);
                throw new StreamDeserializationException(requestContent, typeof(TResponseDto), e);
            }

            LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: true, requestContent);

            return responseDto;
        }

        protected async Task<TResponseDto> Post<TResponseDto>(string endpoint, HttpContent request)
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint);

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await _httpClient.PostAsync(uri, request);
            }
            catch (Exception e)
            {
                _logs.Exception(e);
                throw;
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: false, request.ToString());

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            try
            {
                var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: true, request.ToString());

                return responseDto;
            }
            catch (Exception e)
            {
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: false, request.ToString());
                throw new StreamDeserializationException(request.ToString(), typeof(TResponseDto), e);
            }
        }

        protected async Task<TResponse> Patch<TRequest, TResponse>(string endpoint, TRequest request)
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint);
            var requestContent = _serializer.Serialize(request);

            var httpResponse = await _httpClient.PatchAsync(uri, requestContent);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: false, requestContent);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            TResponse responseDto;

            try
            {
                responseDto = _serializer.Deserialize<TResponse>(responseContent);
            }
            catch (Exception e)
            {
                LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: false, requestContent);
                throw new StreamDeserializationException(requestContent, typeof(TResponse), e);
            }

            LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: true, requestContent);

            return responseDto;
        }

        protected async Task<TResponseDto> Delete<TResponseDto>(string endpoint, Dictionary<string, string> parameters = null)
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint, parameters);

            var httpResponse = await _httpClient.DeleteAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                LogRestCall(uri, endpoint, HttpMethod.Delete, responseContent, success: false);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            LogRestCall(uri, endpoint, HttpMethod.Delete, responseContent, success: true);

            //Todo: wrap in deserialize?
            return _serializer.Deserialize<TResponseDto>(responseContent);
        }

        protected Task PostEventAsync<TEvent, TEventDto>(string channelType, string channelId, TEvent eventBody)
            where TEvent : ISavableTo<TEventDto> =>
            Post<SendEventRequest<TEvent, TEventDto>, SendEventRequestDTO, ApiResponse, ResponseDTO>(
                $"/channels/{channelType}/{channelId}/event", new SendEventRequest<TEvent, TEventDto>
                {
                    Event = eventBody,
                });

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly StringBuilder _sb = new StringBuilder();

        private void LogRestCall(Uri uri, string endpoint, HttpMethod httpMethod, string response, bool success, string request = null)
        {
            _sb.Clear();
            _sb.Append("API Call: ");
            _sb.Append(httpMethod);
            _sb.Append(" ");
            _sb.Append(endpoint);
            _sb.Append(Environment.NewLine);
            _sb.Append("Status: ");
            _sb.Append(success ? "<color=green>SUCCESS</color>" : "<color=red>FAILURE</color>");
            _sb.Append(Environment.NewLine);
            _sb.Append("Full uri: ");
            _sb.Append(uri);
            _sb.Append(Environment.NewLine);
            _sb.Append(Environment.NewLine);

            if (request != null)
            {
                _sb.AppendLine("Request:");
                _sb.AppendLine(request);
                _sb.Append(Environment.NewLine);
            }

            _sb.AppendLine("Response:");
            _sb.AppendLine(response);
            _sb.Append(Environment.NewLine);

            _logs.Info(_sb.ToString());
        }
    }
}