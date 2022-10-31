using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.SampleProject_StateClient.Utils
{
    /// <summary>
    /// Loads images from web
    /// </summary>
    public interface IImageLoader
    {
        Task<Sprite> LoadImageAsync(string url);
    }
}