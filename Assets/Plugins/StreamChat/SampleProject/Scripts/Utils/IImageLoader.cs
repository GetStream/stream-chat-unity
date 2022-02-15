using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.SampleProject.Scripts.Utils
{
    /// <summary>
    /// Loads images from web
    /// </summary>
    public interface IImageLoader
    {
        Task<Sprite> LoadImageAsync(string url);
    }
}