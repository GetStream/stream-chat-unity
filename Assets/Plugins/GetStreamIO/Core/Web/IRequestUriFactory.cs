using System;
using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Web
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    public interface IRequestUriFactory
    {
        Uri CreateConnectionUri();

        Uri CreateEndpointUri(string endpoint, Dictionary<string, string> parameters = null);
    }
}