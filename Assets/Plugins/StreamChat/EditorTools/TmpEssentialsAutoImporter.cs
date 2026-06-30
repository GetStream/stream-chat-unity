using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreamChat.EditorTools
{
    /// <summary>
    /// Ensures TextMesh Pro is available in the project across every supported Unity version
    /// (Unity 2019 → Unity 6.x).
    ///
    /// <c>Packages/manifest.json</c> is NOT committed (it is Unity-version-specific). Each Unity
    /// version generates its own manifest on first open. If the TMP package is missing, this
    /// script prompts the user to install <c>com.unity.textmeshpro</c> via UPM (unpinned — UPM
    /// picks the right version for the current editor).
    ///
    /// Once the TMP package is present, the script also prompts the user to import the
    /// "TMP Essential Resources" (shaders, default fonts, settings) which are likewise
    /// Unity-version-specific and must not be committed to version control.
    ///
    /// IMPORTANT: This file deliberately uses reflection for every TMP API call so that the
    /// <c>StreamChat.EditorTools</c> assembly always compiles even when the TMP package is
    /// missing. If we referenced <c>TMPro.*</c> types directly, this assembly would fail to
    /// compile on a fresh checkout, and <see cref="InitializeOnLoadAttribute"/> would never run —
    /// breaking the very recovery path that's supposed to fix the missing package.
    /// </summary>
    [InitializeOnLoad]
    internal static class TmpEssentialsAutoImporter
    {
        private const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        private const string SampleProjectAsmdefName = "StreamChat.SampleProject";

        private const string SessionKey = "StreamChat_TmpEssentialsImportChecked";
        private const string SkipImportPrefKey = "StreamChat_SkipTmpEssentialsImport";
        private const string SkipPackageInstallPrefKey = "StreamChat_SkipTmpPackageInstall";

        private const string LegacyTmpPackageId = "com.unity.textmeshpro";
        private const string TmpEditorAssemblyName = "Unity.TextMeshPro.Editor";
        private const string TmpRuntimeAssemblyName = "Unity.TextMeshPro";
        private const string TmpEssentialsPackageName = "TMP Essential Resources";
        private const string LogPrefix = "[Stream Chat] ";

        private static AddRequest _addRequest;

        static TmpEssentialsAutoImporter()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += CheckTmpStatus;
        }

        [MenuItem("Tools/Stream Chat/Import TMP Essential Resources")]
        public static void ImportTmpEssentialsMenuItem()
        {
            EditorPrefs.DeleteKey(SkipImportPrefKey);
            EditorPrefs.DeleteKey(SkipPackageInstallPrefKey);
            SessionState.SetBool(SessionKey, false);

            CheckTmpStatus();
        }

        private static void CheckTmpStatus()
        {
            if (!IsSampleProjectPresent())
                return;

            if (!IsTmpPackageInstalled())
            {
                MaybeInstallTmpPackage();
                return;
            }

            if (File.Exists(TmpSettingsPath))
                return;

            MaybeImportTmpEssentials();
        }

        private static bool IsSampleProjectPresent()
        {
            var guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset " + SampleProjectAsmdefName);
            return guids.Length > 0;
        }

        /// <summary>
        /// We can't depend on <c>TMPro</c> types or on a known package version (the latter changes
        /// per Unity release). Instead, detect whether any TMP-providing assembly has been loaded
        /// into the editor's AppDomain.
        /// </summary>
        private static bool IsTmpPackageInstalled()
            => AppDomain.CurrentDomain.GetAssemblies()
                .Any(a =>
                {
                    var name = a.GetName().Name;
                    return name == TmpEditorAssemblyName || name == TmpRuntimeAssemblyName;
                });

        private static void MaybeInstallTmpPackage()
        {
            if (EditorPrefs.GetBool(SkipPackageInstallPrefKey, false))
                return;

            var install = EditorUtility.DisplayDialog(
                "Stream Chat - TextMesh Pro package missing",
                "The Stream Chat Sample Project requires the TextMesh Pro package, " +
                "which is not installed in this project.\n\n" +
                "Would you like to install '" + LegacyTmpPackageId + "' now?\n\n" +
                "After install, Unity will recompile and you'll be prompted to import the " +
                "TMP Essential Resources.",
                "Install", "Skip");

            if (!install)
            {
                EditorPrefs.SetBool(SkipPackageInstallPrefKey, true);
                return;
            }

            Debug.Log(LogPrefix + "Adding " + LegacyTmpPackageId +
                      " package via Unity Package Manager. The editor will recompile when finished.");

            // Unpinned: UPM picks the latest version compatible with this editor (TMP 1.x for
            // Unity 2018, 2.x for Unity 2019.2+, 3.x for Unity 2020+ incl. Unity 6).
            _addRequest = Client.Add(LegacyTmpPackageId);
            EditorApplication.update += MonitorPackageInstall;

            // Force re-check on the next domain reload (after the install/recompile) so we can
            // proceed straight to the essentials-import step without a manual menu click.
            SessionState.SetBool(SessionKey, false);
        }

        private static void MonitorPackageInstall()
        {
            if (_addRequest == null || !_addRequest.IsCompleted)
                return;

            EditorApplication.update -= MonitorPackageInstall;

            if (_addRequest.Status == StatusCode.Success)
            {
                Debug.Log(LogPrefix + "Installed package: " + _addRequest.Result.packageId);
                // Unity triggers a recompile/domain reload automatically; the static ctor will
                // re-run and continue with the essentials-import flow.
            }
            else
            {
                Debug.LogError(LogPrefix + "Failed to install " + LegacyTmpPackageId + ": " +
                               _addRequest.Error?.message);
            }

            _addRequest = null;
        }

        private static void MaybeImportTmpEssentials()
        {
            if (EditorPrefs.GetBool(SkipImportPrefKey, false))
                return;

            var import = EditorUtility.DisplayDialog(
                "Stream Chat - TMP Essential Resources",
                "The Stream Chat Sample Project requires TextMesh Pro Essential Resources, " +
                "which are not yet imported into this project.\n\n" +
                "Would you like to import them now?\n\n" +
                "You can also do this later via:\n" +
                "Tools > Stream Chat > Import TMP Essential Resources",
                "Import", "Skip");

            if (!import)
            {
                EditorPrefs.SetBool(SkipImportPrefKey, true);
                return;
            }

            ImportEssentialsViaReflection();
        }

        private static void ImportEssentialsViaReflection()
        {
            Debug.Log(LogPrefix + "Importing TMP Essential Resources...");

            AssetDatabase.importPackageCompleted += OnTmpEssentialsImported;
            AssetDatabase.importPackageFailed += OnTmpEssentialsImportFailed;

            var importerType = FindTmpType("TMPro.TMP_PackageResourceImporter");
            if (importerType == null)
            {
                UnsubscribeImportCallbacks();
                LogManualFallback("TMPro.TMP_PackageResourceImporter type not found");
                return;
            }

            // ImportResources(bool importEssentials, bool importExamples, bool interactive)
            var method = importerType.GetMethod(
                "ImportResources",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(bool), typeof(bool), typeof(bool) },
                null);

            if (method == null)
            {
                UnsubscribeImportCallbacks();
                LogManualFallback(
                    "TMP_PackageResourceImporter.ImportResources(bool, bool, bool) not found");
                return;
            }

            method.Invoke(null, new object[] { /* importEssentials */ true,
                /* importExamples */ false, /* interactive */ false });
        }

        private static void OnTmpEssentialsImported(string packageName)
        {
            if (packageName != TmpEssentialsPackageName)
                return;

            UnsubscribeImportCallbacks();
            Debug.Log(LogPrefix + "TMP Essential Resources imported. Refreshing assets...");

            EditorApplication.delayCall += RefreshAfterImport;
        }

        private static void RefreshAfterImport()
        {
            AssetDatabase.Refresh();

            var eventManagerType = FindTmpType("TMPro.TMPro_EventManager");
            var notify = eventManagerType?.GetMethod(
                "ON_RESOURCES_LOADED",
                BindingFlags.Public | BindingFlags.Static);
            notify?.Invoke(null, null);

            // TMP components that loaded before the shaders existed have cached broken material
            // state. Reopening the active scene forces all GameObjects to be re-instantiated from
            // their prefabs with the now-correct shader references.
            EditorApplication.delayCall += ReloadActiveScene;
        }

        private static void ReloadActiveScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(scene.path))
                return;

            if (scene.isDirty && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(scene.path);
            Debug.Log(LogPrefix + "TMP Essential Resources are ready.");
        }

        private static void OnTmpEssentialsImportFailed(string packageName, string errorMessage)
        {
            if (packageName != TmpEssentialsPackageName)
                return;

            UnsubscribeImportCallbacks();
            Debug.LogError(
                LogPrefix + "Failed to import TMP Essential Resources: " + errorMessage +
                "\nPlease import them manually via Window > TextMeshPro > Import TMP Essential Resources.");
        }

        private static void UnsubscribeImportCallbacks()
        {
            AssetDatabase.importPackageCompleted -= OnTmpEssentialsImported;
            AssetDatabase.importPackageFailed -= OnTmpEssentialsImportFailed;
        }

        private static Type FindTmpType(string fullTypeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name;
                if (name != TmpEditorAssemblyName && name != TmpRuntimeAssemblyName)
                    continue;

                var type = assembly.GetType(fullTypeName, throwOnError: false);
                if (type != null)
                    return type;
            }

            return null;
        }

        private static void LogManualFallback(string reason)
        {
            Debug.LogError(
                LogPrefix + "Could not auto-import TMP Essential Resources (" + reason + "). " +
                "Please import them manually via Window > TextMeshPro > Import TMP Essential Resources.");
        }
    }
}
