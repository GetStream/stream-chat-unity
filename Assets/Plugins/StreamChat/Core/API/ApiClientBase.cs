using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
                LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: false);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: true);

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
                LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: false);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            LogRestCall(uri, endpoint, HttpMethod.Get, responseContent, success: true);

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Post<TRequest, TRequestDto, TResponse, TResponseDto>(string endpoint,
            TRequest request)
            where TRequest : ISavableTo<TRequestDto>
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
        {
            var uri = _requestUriFactory.CreateEndpointUri(endpoint);
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

            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected async Task<TResponse> Post<TResponse, TResponseDto>(string endpoint, HttpContent request)
            where TResponse : ILoadableFrom<TResponseDto, TResponse>, new()
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

            TResponseDto responseDto;

            try
            {
                responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            }
            catch (Exception e)
            {
                LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: false, request.ToString());
                throw new StreamDeserializationException(request.ToString(), typeof(TResponseDto), e);
            }

            LogRestCall(uri, endpoint, HttpMethod.Post, responseContent, success: true, request.ToString());

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
                LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: false, requestContent);

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
                LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: false, requestContent);
                throw new StreamDeserializationException(requestContent, typeof(TResponseDto), e);
            }

            LogRestCall(uri, endpoint, new HttpMethod("PATCH"), responseContent, success: true, requestContent);

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
                LogRestCall(uri, endpoint, HttpMethod.Delete, responseContent, success: false);

                var apiError = _serializer.Deserialize<APIErrorDTO>(responseContent);
                throw new StreamApiException(apiError);
            }

            LogRestCall(uri, endpoint, HttpMethod.Delete, responseContent, success: true);

            var responseDto = _serializer.Deserialize<TResponseDto>(responseContent);
            var response = new TResponse();
            response.LoadFromDto(responseDto);

            return response;
        }

        protected void LogRestCall(Uri uri, string endpoint, HttpMethod httpMethod, string response, bool success, string request = null)
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

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly StringBuilder _sb = new StringBuilder();
    }
}