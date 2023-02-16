using System;
using System.IO;
using System.Text.RegularExpressions;
using StreamChat.Libs.Utils;
using UnityEditor;

namespace StreamChat.EditorTools.Builders
{
    public class StreamPackageExporter
    {
        /// <summary>
        /// Export StreamChat SDK as .unitypackage
        /// </summary>
        /// <param name="targetDirectory">Full path of target directory</param>
        /// <param name="version">New Version</param>
        /// <param name="changelog">Changelog associated with this particular version</param>
        public void Export(string targetDirectory, Version version, string changelog, out string packagePath)
        {
            if (changelog.IsNullOrEmpty())
            {
                throw new ArgumentException($"{nameof(changelog)} cannot be empty");
            }

            if (targetDirectory.IsNullOrEmpty())
            {
                throw new ArgumentException($"{nameof(targetDirectory)} cannot be empty");
            }

            GetFiles(out var streamChatClientFilePath, out var changelogFilepath);

            var currentVersion = GetCurrentVersion(streamChatClientFilePath);
            if (currentVersion >= version)
            {
                throw new InvalidOperationException($"New version: {version} must be greater than the current: {currentVersion}");
            }

            SetStreamChatClientVersion(streamChatClientFilePath, version);
            SetPlayerSettingsVersion(version);

            UpdateChangeLog(changelog, changelogFilepath, version);

            var pluginDirectory = Path.GetDirectoryName(changelogFilepath);

            packagePath = ExportPackage(targetDirectory, pluginDirectory, version);
        }

        private const string StreamChatLowLevelClientFilename = "StreamChatLowLevelClient.cs";
        private const string VersionRegexPattern = @"SDKVersion\s+=\s+new\s+Version\(\s*([0-9]+),\s*([0-9]+),\s*([0-9]+)\)\s*;";
        private const string VersionLineTemplate =
            "        public static readonly Version SDKVersion = new Version({0}, {1}, {2});";

        private const string ChangelogFilename = "Changelog.txt";
        private const string PackageNameTemplate = "Stream.Chat.Unity.SDK.{0}.unitypackage";

        private static void GetFiles(out string streamChatClientFilepath, out string changelogFilepath)
        {
            var assetPaths = AssetDatabase.GetAllAssetPaths();

            streamChatClientFilepath = null;
            changelogFilepath = null;

            foreach (var file in assetPaths)
            {
                if (Path.GetFileName(file) == StreamChatLowLevelClientFilename)
                {
                    streamChatClientFilepath = file;
                }

                if (Path.GetFileName(file) == ChangelogFilename)
                {
                    changelogFilepath = file;
                }
            }

            if (streamChatClientFilepath.IsNullOrEmpty())
            {
                throw new FileNotFoundException($"Failed to find `{StreamChatLowLevelClientFilename}` file");
            }

            if (changelogFilepath.IsNullOrEmpty())
            {
                throw new FileNotFoundException($"Failed to find `{ChangelogFilename}` file");
            }
        }

        private static Version GetCurrentVersion(string streamChatClientFilePath)
        {
            var content = File.ReadAllText(streamChatClientFilePath);

            var match = Regex.Match(content, VersionRegexPattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new InvalidOperationException("Failed to find version line");
            }

            int GetComponent(int index)
            {
                if (match.Groups.Count <= index)
                {
                    throw new InvalidOperationException("Failed to find version component with index: " + index);
                }

                var value = match.Groups[index].Value;
                if (!int.TryParse(value, out var versionComponent))
                {
                    throw new InvalidOperationException(
                        $"Version component with index `{index}` is not a valid integer. Received value: " + value);
                }

                return versionComponent;
            }

            return new Version(GetComponent(1), GetComponent(2), GetComponent(3));
        }

        private static void SetPlayerSettingsVersion(Version version)
            => PlayerSettings.bundleVersion = version.ToString();

        private static void SetStreamChatClientVersion(string filePath, Version version)
        {
            var allLines = File.ReadAllLines(filePath);
            using (var output = new StreamWriter(filePath))
            {
                foreach (var line in allLines)
                {
                    var match = Regex.Match(line, VersionRegexPattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var versionLine = string.Format(VersionLineTemplate, version.Major, version.Minor, version.Build);
                        output.WriteLine(versionLine);
                        continue;
                    }
                    output.WriteLine(line);
                }
            }
        }

        private static void UpdateChangeLog(string versionChangelog, string changeLogFilepath, Version version)
        {
            var changeLogFileContent = File.ReadAllText(changeLogFilepath);

            var trimmedVersionChangelog = versionChangelog.Trim();
            if (trimmedVersionChangelog.StartsWith($"v{version.Major}"))
            {
                throw new ArgumentException("Version changelog should not contain the version keyword at the beginning");
            }

            var versionKey = version.ToString();

            using (var output = new StreamWriter(changeLogFilepath))
            {
                output.WriteLine($"v{versionKey}:");
                output.Write(trimmedVersionChangelog);
                output.WriteLine(Environment.NewLine);
                output.Write(changeLogFileContent.Trim());
            }
        }

        private static string ExportPackage(string targetDirectory, string rooDirectoryPath, Version version)
        {
            var filename = string.Format(PackageNameTemplate, version);
            var path = Path.Combine(targetDirectory, filename);
            AssetDatabase.ExportPackage(rooDirectoryPath, path, ExportPackageOptions.Recurse);

            return path;
        }
    }
}