using UnityEngine.UIElements;

namespace StreamChat.SampleProjects.UIToolkit.Utils
{
    public static class UIToolkitUtils
    {
        public static void RemoveAllChildren(this VisualElement vs)
        {
            for (var i = vs.childCount - 1; i >= 0; i--)
            {
                vs.RemoveAt(i);
            }
        }
    }
}