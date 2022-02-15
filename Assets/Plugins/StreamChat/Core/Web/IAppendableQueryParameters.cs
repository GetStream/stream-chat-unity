namespace StreamChat.Core.Web
{
    internal interface IAppendableQueryParameters
    {
        void AppendQueryParameters(QueryParameters queryParameters);
    }
}