using UnityEngine.Networking;

namespace StreamChat.Libs.Utils
{
    internal static class UnityWebRequestExt
    {
        public static bool IsRequestSuccessful(this UnityWebRequest unityWebRequest)
        {
#if UNITY_2020_1_OR_NEWER
            return unityWebRequest.result == UnityWebRequest.Result.Success;
#else
            return string.IsNullOrEmpty(unityWebRequest.error);
#endif
        }
    }
}