﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StreamChat.Core.Exceptions;
using StreamChat.Core.InternalDTO.Models;
using StreamChat.Core.InternalDTO.Requests;
using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.Web;
using StreamChat.Libs.Http;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Serialization;

namespace StreamChat.Core.LowLevelClient.API.Internal
{
    /// <summary>
    /// Base Api client
    /// </summary>
    internal abstract class InternalApiClientBase
    {
        protected InternalApiClientBase(IHttpClient httpClient, ISerializer serializer, ILogs logs,
            IRequestUriFactory requestUriFactory, IStreamChatLowLevelClient lowLevelClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _requestUriFactory = requestUriFactory ?? throw new ArgumentNullException(nameof(requestUriFactory));
            _lowLevelClient = lowLevelClient ?? throw new ArgumentNullException(nameof(lowLevelClient));
        }

        protected Task<TResponse> Get<TPayload, TResponse>(string endpoint, TPayload payload)
            => HttpRequest<TResponse>(HttpMethod.Get, endpoint, payload);

        protected Task<TResponse> Get<TResponse>(string endpoint, QueryParameters parameters = null)
            => HttpRequest<TResponse>(HttpMethod.Get, endpoint, queryParameters: parameters);

        protected Task<TResponse> Post<TRequest, TResponse>(string endpoint, TRequest request)
            => HttpRequest<TResponse>(HttpMethod.Post, endpoint, request);

        protected Task<TResponse> Post<TResponse>(string endpoint, HttpContent request)
            => HttpRequest<TResponse>(HttpMethod.Post, endpoint, request);

        protected Task<TResponse> Put<TRequest, TResponse>(string endpoint, TRequest request)
            => HttpRequest<TResponse>(HttpMethod.Put, endpoint, request);

        protected Task<TResponse> Patch<TRequest, TResponse>(string endpoint, TRequest request)
            => HttpRequest<TResponse>(PatchHttpMethod, endpoint, request);

        protected Task<TResponse> Delete<TResponse>(string endpoint, QueryParameters parameters = null)
            => HttpRequest<TResponse>(HttpMethod.Delete, endpoint, queryParameters: parameters);

        protected Task PostEventAsync(string channelType, string channelId, object eventBodyDto)
            => Post<SendEventRequestInternalDTO, ResponseInternalDTO>(
                $"/channels/{channelType}/{channelId}/event", new SendEventRequestInternalDTO
                {
                    Event = eventBodyDto,
                });

        private const int InvalidAuthTokenErrorCode = 40;

        private static readonly HttpMethod PatchHttpMethod = new HttpMethod("PATCH");

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogs _logs;
        private readonly IRequestUriFactory _requestUriFactory;
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly IStreamChatLowLevelClient _lowLevelClient;

        private HttpContent TryGetHttpContent(object content, out string serializedContent)
        {
            serializedContent = default;

            if (content == null)
            {
                return null;
            }

            if (content is HttpContent httpContent)
            {
                return httpContent;
            }

            serializedContent = _serializer.Serialize(content);
            return new StringContent(serializedContent);
        }

        private async Task<TResponse> HttpRequest<TResponse>(HttpMethod httpMethod, string endpoint,
            object requestBody = default, QueryParameters queryParameters = null)
        {
            //StreamTodo: perhaps remove this requirement, sometimes we send empty body without any properties
            if (requestBody == null && IsRequestBodyRequiredByHttpMethod(httpMethod))
            {
                throw new ArgumentException($"{nameof(requestBody)} is required by {httpMethod}");
            }

            var httpContent = TryGetHttpContent(requestBody, out var serializedContent);
            var logContent = serializedContent ?? httpContent?.ToString();

            if (httpMethod == HttpMethod.Get && serializedContent != null)
            {
                queryParameters ??= QueryParameters.Default;
                queryParameters.Append("payload", serializedContent);
            }

            var uri = _requestUriFactory.CreateEndpointUri(endpoint, queryParameters);

            LogFutureRequestIfDebug(uri, endpoint, httpMethod, logContent);

            var httpResponse = await ExecuteHttpMethodAsync(httpMethod, uri, httpContent);
            var responseContent = httpResponse.Result;

            if (!httpResponse.IsSuccessStatusCode)
            {
                var apiError = _serializer.Deserialize<APIErrorInternalDTO>(responseContent);

                if (apiError.Code != InvalidAuthTokenErrorCode)
                {
                    LogRestCall(uri, endpoint, httpMethod, responseContent, success: false, logContent);
                    throw new StreamApiException(apiError);
                }

                if (_lowLevelClient.ConnectionState == ConnectionState.Connected)
                {
                    _logs.Info("INTERCEPTOR - DisconnectAsync connection_id = " +
                               ((IConnectionProvider)_lowLevelClient).ConnectionId);
                    await _lowLevelClient.DisconnectAsync();
                }

                _logs.Info("INTERCEPTOR - New Token required, connection: " + _lowLevelClient.ConnectionState);

                const int maxMsToWait = 500;
                var i = 0;
                
                //StreamTodo: we can create cancellation token instead of Task.Delay in loop
                while (_lowLevelClient.ConnectionState != ConnectionState.Connected)
                {
                    i++;
                    await Task.Delay(1);

                    if (i > maxMsToWait)
                    {
                        break;
                    }
                }

                if (_lowLevelClient.ConnectionState != ConnectionState.Connected)
                {
                    throw new TimeoutException(
                        "Request reached timout when waiting for client to reconnect after auth token refresh");
                }

                // Recreate the uri to include new connection id 
                uri = _requestUriFactory.CreateEndpointUri(endpoint, queryParameters);

                httpResponse = await ExecuteHttpMethodAsync(httpMethod, uri, httpContent);
                responseContent = httpResponse.Result;
            }

            try
            {
                var response = _serializer.Deserialize<TResponse>(responseContent);
                LogRestCall(uri, endpoint, httpMethod, responseContent, success: true, logContent);
                return response;
            }
            catch (Exception e)
            {
                LogRestCall(uri, endpoint, httpMethod, responseContent, success: false, logContent);
                throw new StreamDeserializationException(responseContent, typeof(TResponse), e);
            }
        }

        private Task<HttpResponse> ExecuteHttpMethodAsync(HttpMethod httpMethod, Uri uri,
            HttpContent requestContent = default)
        {
            if (httpMethod == HttpMethod.Get)
            {
                return _httpClient.GetAsync(uri);
            }

            if (httpMethod == HttpMethod.Post)
            {
                return _httpClient.PostAsync(uri, requestContent);
            }

            if (httpMethod == HttpMethod.Delete)
            {
                return _httpClient.DeleteAsync(uri);
            }

            if (httpMethod == HttpMethod.Put)
            {
                return _httpClient.PutAsync(uri, requestContent);
            }

            if (httpMethod == PatchHttpMethod)
            {
                return _httpClient.PatchAsync(uri, requestContent);
            }

            throw new InvalidOperationException($"Method {httpMethod} is not supported");
        }

        private static bool IsRequestBodyRequiredByHttpMethod(HttpMethod httpMethod)
            => httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == PatchHttpMethod;

        private void LogFutureRequestIfDebug(Uri uri, string endpoint, HttpMethod httpMethod, string request = null)
        {
#if STREAM_DEBUG_ENABLED
            _sb.Clear();

            _sb.Clear();
            _sb.Append("API Call: ");
            _sb.Append(httpMethod);
            _sb.Append(" ");
            _sb.Append(endpoint);
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

            _logs.Info(_sb.ToString());
#endif
        }

        private void LogRestCall(Uri uri, string endpoint, HttpMethod httpMethod, string response, bool success,
            string request = null)
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