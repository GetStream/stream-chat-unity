using System;
using System.Collections.Generic;

namespace StreamChat.Core.Web
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    internal interface IRequestUriFactory
    {
        Uri CreateConnectionUri();

        Uri CreateEndpointUri(string endpoint, Dictionary<string, string> parameters = null);
    }
}