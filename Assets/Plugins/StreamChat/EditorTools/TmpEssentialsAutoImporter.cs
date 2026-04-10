using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StreamChat.EditorTools
{
    /// <summary>
    /// Prompts to import TMP Essential Resources when the Sample Project is present but
    /// TMP Essentials are missing. TMP Essentials (shaders, default fonts, settings) are
    /// Unity-version-specific and must not be committed to version control.
    /// After importing, reloads the active scene so all TMP components reinitialize
    /// with correct materials.
    /// </summary>
    [InitializeOnLoad]
    internal static class TmpEssentialsAutoImporter
    {
        private const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        private const string SampleProjectAsmdefName = "StreamChat.SampleProject";
        private const string SessionKey = "StreamChat_TmpEssentialsImportChecked";
        private const string SkipImportPrefKey = "StreamChat_SkipTmpEssentialsImport";

        static TmpEssentialsAutoImporter()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += CheckAndImportTmpEssentials;
        }

        [MenuItem("Tools/Stream Chat/Import TMP Essential Resources")]
        public static void ImportTmpEssentialsMenuItem()
        {
            EditorPrefs.DeleteKey(SkipImportPrefKey);
            SessionState.SetBool(SessionKey, false);

            if (File.Exists(TmpSettingsPath))
            {
                return;
            }

            PerformImport();
        }

        private static void CheckAndImportTmpEssentials()
        {
            if (File.Exists(TmpSettingsPath))
                return;

            if (!IsSampleProjectPresent())
                return;

            if (EditorPrefs.GetBool(SkipImportPrefKey, false))
                return;

            bool import = EditorUtility.DisplayDialog(
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

            PerformImport();
        }

        private static bool IsSampleProjectPresent()
        {
            var guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset " + SampleProjectAsmdefName);
            return guids.Length > 0;
        }

        private static void PerformImport()
        {
            Debug.Log("[Stream Chat] Importing TMP Essential Resources...");

            AssetDatabase.importPackageCompleted += OnTmpEssentialsImported;
            AssetDatabase.importPackageFailed += OnTmpEssentialsImportFailed;

            TMPro.TMP_PackageResourceImporter.ImportResources(
                importEssentials: true,
                importExamples: false,
                interactive: false);
        }

        private static void OnTmpEssentialsImported(string packageName)
        {
            if (packageName != "TMP Essential Resources")
                return;

            UnsubscribeCallbacks();
            Debug.Log("[Stream Chat] TMP Essential Resources imported. Refreshing assets...");

            EditorApplication.delayCall += RefreshAfterImport;
        }

        private static void RefreshAfterImport()
        {
            AssetDatabase.Refresh();
            TMPro.TMPro_EventManager.ON_RESOURCES_LOADED();

            // TMP components that loaded before the shaders existed have cached broken material
            // state. Reopening the scene forces all GameObjects to be re-instantiated from their
            // prefabs with the now-correct shader references.
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
            Debug.Log("[Stream Chat] TMP Essential Resources are ready.");
        }

        private static void OnTmpEssentialsImportFailed(string packageName, string errorMessage)
        {
            if (packageName != "TMP Essential Resources")
                return;

            UnsubscribeCallbacks();
            Debug.LogError(
                "[Stream Chat] Failed to import TMP Essential Resources: " + errorMessage +
                "\nPlease import them manually via Window > TextMeshPro > Import TMP Essential Resources.");
        }

        private static void UnsubscribeCallbacks()
        {
            AssetDatabase.importPackageCompleted -= OnTmpEssentialsImported;
            AssetDatabase.importPackageFailed -= OnTmpEssentialsImportFailed;
        }
    }
}
