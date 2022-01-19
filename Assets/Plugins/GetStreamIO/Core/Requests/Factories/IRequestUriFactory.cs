using System;
using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Requests.Factories
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