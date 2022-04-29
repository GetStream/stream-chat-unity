using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Core.DTO.Models;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;
using StreamChat.Core.Exceptions;
using StreamChat.Core.Web;

namespace StreamChat.Core.API
{
    //Todo: refactor methods to remove duplication
    //Probably best to use HttpClient.SendAsync only with optional content instead specialized methods that have common logic
    //We could also not pass TRequestDto TResponseDto but have it registered in a map so that calls are not bloated with so many types

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

        protected async Task<TResponse> Get<TResponse, TResponseDto>(string endpoint,
            Dictionary<string, string> parameters = null)
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint, parameters);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Get<TPayload, TPayloadDTO, TResponse, TResponseDto>(string endpoint, TPayload payload)
            where TPayload : ISavableTo<TPayloadDTO>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var payloadContent = _serializer.Serialize(payload.SaveToDto());
            var parameters = QueryParameters.Default.Append("payload", payloadContent);

            var uri = _requestUriFactory.CreateEndpointUri(endpoint, parameters);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Post<TRequest, TRequestDto, TResponse, TResponseDto>(string url,
            TRequest request)
            where TRequest : ISavableTo<TRequestDto>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(url);
            var requestContent = _serializer.Serialize(request.SaveToDto());

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
                throw new StreamDeserializationException(requestContent, typeof(TResponseDto), e);
            }

            LogRestCall(uri, requestContent, responseContent);

            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Post<TResponse, TResponseDto>(string url, HttpContent request)
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(url);

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
                throw new StreamDeserializationException(request.ToString(), typeof(TResponseDto), e);
            }

            LogRestCall(uri, request.ToString(), responseContent);

            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Patch<TRequest, TRequestDto, TResponse, TResponseDto>(string endpoint,
            TRequest request)
            where TRequest : ISavableTo<TRequestDto>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint);
            var requestContent = _serializer.Serialize(request.SaveToDto());

            var httpResponse = await _httpClient.PatchAsync(uri, requestContent);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
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
                throw new StreamDeserializationException(requestContent, typeof(TResponseDto), e);
            }

            LogRestCall(uri, requestContent, responseContent);

            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Delete<TResponse, TResponseDto>(string endpoint,
            Dictionary<string, string> parameters = null)
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint, parameters);

            var httpResponse = await _httpClient.DeleteAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected void LogRestCall(Uri uri, string request, string response)
            => _logs.Info($"REST API Call: {uri}\n\nRequest:\n{request}\n\nResponse:\n{response}\n\n\n");

        protected void LogRestCall(Uri uri, string response)
            => _logs.Info($"REST API Call: {uri}\n\nResponse:\n{response}\n\n\n");

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly IRequestUriFactory _requestUriFactory;

        protected ISerializer Serializer => _serializer;
    }
}