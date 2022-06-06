using System.Collections.Generic;
using UnityEditor;

namespace StreamChat.Editor.DefineSymbols
{
    public interface IUnityDefineSymbols
    {
        string[] GetScriptingDefineSymbols(BuildTarget buildTarget);

        void SetScriptingDefineSymbols(BuildTarget buildTarget, ICollection<string> defines);
    }
}