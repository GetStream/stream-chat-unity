using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StreamChat.Libs.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StreamChat.EditorTools
{
    public class SpriteAtlasUtilityEditor : EditorWindow
    {
        protected void OnGUI()
        {
            GUILayout.Label("Sprites Atlas Utility", EditorStyles.boldLabel);
            GUILayout.Label("This tool will apply names to sprite atlas from external key:name mapping json");

            _spriteAtlasTexture = EditorGUILayout.ObjectField("TMP Sprite Asset:", _spriteAtlasTexture,
                typeof(Texture2D),
                allowSceneObjects: false);
            _jsonMapping = EditorGUILayout.ObjectField("Json Mapping Text File:", _jsonMapping, typeof(TextAsset),
                allowSceneObjects: false);

            if (!string.IsNullOrEmpty(_error))
            {
                using (new GUIColorScope(Color.red))
                {
                    GUILayout.Label(_error, EditorStyles.boldLabel);
                }
            }
            else if (!string.IsNullOrEmpty(_log))
            {
                using (new GUIColorScope(Color.green))
                {
                    GUILayout.Label(_log, EditorStyles.boldLabel);
                }
            }

            if (GUILayout.Button("Apply"))
            {
                _error = string.Empty;

                if (_spriteAtlasTexture == null)
                {
                    _error = $"Sprite Sheet Atlas cannot be null";
                    return;
                }

                if (!(_spriteAtlasTexture is Texture2D spriteAtlasTexture))
                {
                    _error = $"Sprite sheet is not of type: {nameof(Texture2D)}";
                    return;
                }

                if (_jsonMapping == null)
                {
                    _error = $"Json Mapping Text File cannot be null";
                    return;
                }

                if (!(_jsonMapping is TextAsset jsonMappingTextAsset))
                {
                    _error = $"Failed to cast {nameof(_jsonMapping)} to {nameof(TextAsset)} type";
                    return;
                }

                var serializer = new NewtonsoftJsonSerializer();
                var mapping = serializer.Deserialize<Dictionary<string, string>>(jsonMappingTextAsset.text);

                var spriteNameToShortcut = new Dictionary<string, string>();

                foreach (var mappingEntry in mapping)
                {
                    if (mappingEntry.Value == null)
                    {
                        continue;
                    }

                    if (spriteNameToShortcut.ContainsKey(mappingEntry.Value))
                    {
                        Debug.LogWarning($"{nameof(spriteNameToShortcut)} already contains key: " + mappingEntry.Value);
                        continue;
                    }

                    spriteNameToShortcut[mappingEntry.Value] = mappingEntry.Key;
                }

                var path = AssetDatabase.GetAssetPath(_spriteAtlasTexture);
                var metaFilePath = path + ".meta";

                var sprites = AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<Sprite>().ToArray();

                var metaFileContents = File.ReadAllText(metaFilePath);

                int changed = 0;
                foreach (var s in sprites)
                {
                    if (!spriteNameToShortcut.ContainsKey(s.name))
                    {
                        continue;
                    }

                    var currentName = s.name;
                    var newName = spriteNameToShortcut[s.name];

                    metaFileContents = metaFileContents.Replace($"name: {currentName}", $"name: {newName}");

                    //Does change names in Unity memory but is not written to file
                    s.name = spriteNameToShortcut[s.name];
                    EditorUtility.SetDirty(s);
                    changed++;
                }

                if (changed > 0)
                {
                    File.WriteAllText(metaFilePath, metaFileContents);

                    EditorUtility.SetDirty(_spriteAtlasTexture);
                    AssetDatabase.SaveAssets();
                    _log =
                        $"Finished. Updated {changed} out of {sprites.Length} sprites in `{_spriteAtlasTexture.name}` atlas";
                }
                else
                {
                    _log = "Finished, but no sprites were changed";
                }
            }
        }

        private Object _spriteAtlasTexture;
        private Object _jsonMapping;
        private string _error;
        private string _log;

        private readonly struct GUIColorScope : IDisposable
        {
            public GUIColorScope(Color color)
            {
                _prev = GUI.color;
                GUI.color = color;
            }

            public void Dispose() => GUI.color = _prev;

            private readonly Color _prev;
        }
    }
}