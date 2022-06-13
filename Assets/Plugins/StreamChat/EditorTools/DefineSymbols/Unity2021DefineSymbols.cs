#if UNITY_2021
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

namespace StreamChat.EditorTools.DefineSymbols
{
    public class Unity2021DefineSymbols : IUnityDefineSymbols
    {
        public string[] GetScriptingDefineSymbols(BuildTarget buildTarget)
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            return symbols.Split(';');
        }

        public void SetScriptingDefineSymbols(BuildTarget buildTarget, ICollection<string> defines)
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            var combinedSymbols = string.Join(";", defines);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, combinedSymbols);
        }
    }
}
#endif