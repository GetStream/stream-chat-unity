namespace StreamChat.Core.Web
{
    internal static class QueryParametersExt
    {
        public static QueryParameters Append(this QueryParameters queryParameters, string key, bool value)
            => Append(queryParameters, key, value.ToString());

        public static QueryParameters Append(this QueryParameters queryParameters, string key, string value)
        {
            queryParameters[key] = value;
            return queryParameters;
        }

        public static QueryParameters AppendFrom(this QueryParameters queryParameters, IAppendableQueryParameters source)
        {
            source.AppendQueryParameters(queryParameters);
            return queryParameters;
        }
    }
}