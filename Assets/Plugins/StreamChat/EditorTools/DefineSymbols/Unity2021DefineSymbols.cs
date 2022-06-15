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
#if UNITY_2021_1
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            return symbols.Split(';');
#else
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            return symbols.Split(';');
#endif


        }

        public void SetScriptingDefineSymbols(BuildTarget buildTarget, ICollection<string> defines)
        {
#if UNITY_2021_1
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
#else

            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            var combinedSymbols = string.Join(";", defines);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, combinedSymbols);
#endif
        }
    }
}
#endif