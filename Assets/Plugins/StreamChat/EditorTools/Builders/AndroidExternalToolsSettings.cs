namespace Plugins.StreamChat.EditorTools.Builders
{
    public readonly struct AndroidExternalToolsSettings
    {
        public string JdkPath { get; }
        public string AndroidSdkPath { get; }
        public string AndroidNdkPath { get; }
        public string GradlePath { get; }

        public AndroidExternalToolsSettings(string jdkPath, string androidSdkPath, string androidNdkPath, string gradlePath)
        {
            JdkPath = jdkPath;
            AndroidSdkPath = androidSdkPath;
            AndroidNdkPath = androidNdkPath;
            GradlePath = gradlePath;
        }

        public override string ToString()
        {
            return $"{nameof(AndroidExternalToolsSettings)} - {nameof(JdkPath)}: {JdkPath}, {nameof(AndroidSdkPath)}: {AndroidSdkPath}, " +
                   $"{nameof(AndroidNdkPath)}: {AndroidNdkPath}, {nameof(GradlePath)}: {GradlePath}";
        }
    }
}