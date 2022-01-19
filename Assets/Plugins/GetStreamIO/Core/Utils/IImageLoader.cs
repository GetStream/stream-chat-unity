using System.Threading.Tasks;
using UnityEngine;

namespace Plugins.GetStreamIO.Core.Utils
{
    /// <summary>
    /// Loads images from web
    /// </summary>
    public interface IImageLoader
    {
        Task<Sprite> LoadImageAsync(string url);
    }
}