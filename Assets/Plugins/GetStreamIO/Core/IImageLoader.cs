using System.Threading.Tasks;
using UnityEngine;

namespace Plugins.GetStreamIO.Core
{
    /// <summary>
    /// Loads images from web
    /// </summary>
    public interface IImageLoader
    {
        Task<Texture2D> LoadImageAsync(string url);
    }
}