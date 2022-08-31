using System;
using System.Text.RegularExpressions;
using StreamChat.EditorTools.Builders;
using StreamChat.Libs.Utils;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools
{
    public class StreamPackageExportEditor : EditorWindow
    {
        protected void OnEnable()
        {
            _targetDirectory = EditorPrefs.GetString(EditorPrefsLastTargetDirectory);
        }

        protected void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Version (major, minor, build):");
                _versionMajor = EditorGUILayout.IntField(_versionMajor);
                _versionMinor = EditorGUILayout.IntField(_versionMinor);
                _versionBuild = EditorGUILayout.IntField(_versionBuild);
            }

            GUILayout.Space(10);

            GUILayout.Label("Changelog:");

            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_textAreaScrollPos, GUILayout.Height(400)))
            {
                _textAreaScrollPos = scrollViewScope.scrollPosition;
                _changelog = EditorGUILayout.TextArea(_changelog, GUILayout.ExpandHeight(true));
            }

            if (GUILayout.Button("Strip changelog markdown"))
            {
                StripChangelogMarkdown();
            }

            GUILayout.Space(10);

            using (new GUILayout.HorizontalScope())
            {
                var targetDirectoryInfo = "Target Directory: " + (_targetDirectory.IsNullOrEmpty() ? "None" : _targetDirectory);
                GUILayout.Label(targetDirectoryInfo, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    _targetDirectory = EditorUtility.OpenFolderPanel("Pick Export Target Directory", _targetDirectory, "");
                }
            }

            GUILayout.Space(10);

            _stripMarkdown = EditorGUILayout.Toggle("Strip markdown from changelog", _stripMarkdown);

            GUILayout.Space(10);

            if (GUILayout.Button("Export"))
            {
                _lastSuccess = false;
                _lastError = null;
                try
                {
                    if (_stripMarkdown)
                    {
                        StripChangelogMarkdown();
                    }
                    _exporter.Export(_targetDirectory, new Version(_versionMajor, _versionMinor, _versionBuild), _changelog, out var packagePath);
                    EditorPrefs.SetString(EditorPrefsLastTargetDirectory, _targetDirectory);

                    _lastSuccess = true;
                    EditorUtility.RevealInFinder(packagePath);
                }
                catch (Exception e)
                {
                    _lastError = e.ToString();
                }
            }

            if (_lastSuccess)
            {
                using (new GUIColorScope(Color.green))
                {
                    GUILayout.Label("Export successful!");
                }
            }

            if (!_lastError.IsNullOrEmpty())
            {
                using (new GUIColorScope(Color.red))
                {
                    GUILayout.Label(_lastError);
                }
            }
        }

        private const string EditorPrefsLastTargetDirectory = "fda978fdsah_LastExportPackageTargetDirectory";

        private readonly StreamPackageExporter _exporter = new StreamPackageExporter();

        private int _versionMajor, _versionMinor, _versionBuild;
        private string _changelog;
        private string _targetDirectory;
        private bool _stripMarkdown = true;

        private string _lastError;
        private bool _lastSuccess;

        private Vector2 _textAreaScrollPos;

        private void StripChangelogMarkdown()
        {
            _changelog = CleanupMarkdown(_changelog);

            // This fixed TextArea above to refresh and properly show cleaned up changelog
            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;
        }

        private static string CleanupMarkdown(string text)
        {
            text = text.Replace("`", "");

            // GH Issues
            text = Regex.Replace(text, @"\(#[0-9]+\)", "", RegexOptions.Multiline);

            // GH Headers
            text = Regex.Replace(text, @"^#+\s*", "", RegexOptions.Multiline);

            // first - to *
            text = Regex.Replace(text, @"^([^-]\s*)?(\-\s*)", "$1* ", RegexOptions.Multiline);

            return text.Trim();
        }
    }
}