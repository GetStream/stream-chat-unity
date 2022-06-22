using UnityEditor;

namespace StreamChat.EditorTools.Builders
{
    public readonly struct BuildSettings
    {
        public BuildTargetGroup BuildTargetGroup { get; }
        public ApiCompatibilityLevel ApiCompatibilityLevel { get; }
        public ScriptingImplementation ScriptingImplementation { get; }
        public string TargetPath { get; }

        public BuildSettings(BuildTargetGroup buildTargetGroup, ApiCompatibilityLevel apiCompatibilityLevel,
            ScriptingImplementation scriptingImplementation, string targetPath)
        {
            BuildTargetGroup = buildTargetGroup;
            ApiCompatibilityLevel = apiCompatibilityLevel;
            ScriptingImplementation = scriptingImplementation;
            TargetPath = targetPath;
        }

        public override string ToString() =>
            $"{nameof(BuildSettings)} - {nameof(BuildTargetGroup)}: {BuildTargetGroup}, {nameof(ApiCompatibilityLevel)}: {ApiCompatibilityLevel}, " +
            $"{nameof(ScriptingImplementation)}: {ScriptingImplementation}, {nameof(TargetPath)}: {TargetPath}";
    }
}