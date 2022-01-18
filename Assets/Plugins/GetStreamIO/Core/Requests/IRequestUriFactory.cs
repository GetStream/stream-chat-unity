using System;
using System.Collections.Generic;

namespace Plugins.GetStreamIO.Core.Requests
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    public interface IRequestUriFactory
    {
        Uri CreateConnectionUri();

        Uri CreateMuteUserUri();

        Uri CreateEndpointUri(string endpoint, Dictionary<string, string> parameters = null);
    }
}