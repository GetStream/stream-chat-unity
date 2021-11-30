using System;
using Plugins.GetStreamIO.Core.Models;

namespace Plugins.GetStreamIO.Core.Requests
{
    /// <summary>
    /// Requests Uri Factory
    /// </summary>
    public interface IRequestUriFactory
    {
        Uri CreateConnectionUri();

        Uri CreateChannelsUri();

        Uri CreateSendMessageUri(Channel channel);
    }
}