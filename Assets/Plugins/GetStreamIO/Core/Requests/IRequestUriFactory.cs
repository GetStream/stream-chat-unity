using System;
using Plugins.GetStreamIO.Core.Models;
using Plugins.GetStreamIO.Core.Models.V2;

namespace Plugins.GetStreamIO.Core.Requests
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    public interface IRequestUriFactory
    {
        Uri CreateConnectionUri();

        Uri CreateDeleteMessageUri(Message message, bool? isHardDelete);

        Uri CreateMuteUserUri();

        Uri CreateUpdateMessageUri(Message message);

        Uri CreateDefaultEndpointUri(string endpoint);
    }
}