﻿using StreamChat.Core.Web;

namespace StreamChat.Core.Web
{
    public interface IAppendableQueryParameters
    {
        void AppendQueryParameters(QueryParameters queryParameters);
    }
}