#if UNITY_2020_1_OR_NEWER && !UNITY_2021
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace StreamChat.EditorTools.DefineSymbols
{
    public class Unity2020DefineSymbols : IUnityDefineSymbols
    {
        public string[] GetScriptingDefineSymbols(BuildTarget buildTarget)
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            return symbols.Split(';');
        }

        public void SetScriptingDefineSymbols(BuildTarget buildTarget, ICollection<string> defines)
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
        }
    }
}
#endif