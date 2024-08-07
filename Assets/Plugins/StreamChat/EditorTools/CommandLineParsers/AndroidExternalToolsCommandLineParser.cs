using System;
using System.Collections.Generic;
using Plugins.StreamChat.EditorTools.Builders;

namespace StreamChat.EditorTools.CommandLineParsers
{
    public class AndroidExternalToolsCommandLineParser : CommandLineParserBase<AndroidExternalToolsSettings>
    {
        public const string JdkPathArgKey = "-jdkPath";
        public const string AndroidSdkPathArgKey = "-androidSdkPath";
        public const string AndroidNdkPathArgKey = "-androidNdkPath";
        public const string GradlePathArgKey = "-gradlePath";

        protected override AndroidExternalToolsSettings Parse(IDictionary<string, string> args)
        {
            if (args.Count == 0)
            {
                throw new ArgumentException($"No arguments provided");
            }

            return new AndroidExternalToolsSettings(GetKeyOrDefault(JdkPathArgKey), GetKeyOrDefault(AndroidSdkPathArgKey), GetKeyOrDefault(AndroidNdkPathArgKey),
                GetKeyOrDefault(GradlePathArgKey));

            string GetKeyOrDefault(string key) => args.TryGetValue(key, out var arg) ? arg : string.Empty;
        }
    }
}