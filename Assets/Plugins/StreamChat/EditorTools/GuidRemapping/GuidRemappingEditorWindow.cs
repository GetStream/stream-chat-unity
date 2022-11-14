using System.Collections.Generic;
using System.IO;
using StreamChat.Libs.Utils;
using UnityEditor;
using UnityEngine;

namespace StreamChat.EditorTools.GuidRemapper
{
    public sealed class GuidRemappingEditorWindow : EditorWindow
    {
        void OnGUI()
        {
            GUILayout.Label("Info:");
            GUILayout.TextArea("This tool will try to map GUIDs from NEW folder with GUIDs from OLD folder based on the component name and path and replace OLD references with NEW references in the TARGET folder ");
            GUILayout.Label("Info:");
            
            GUILayout.Label("OLD folder:: " + PrintFolderIsSet(_oldReferences));
            if (GUILayout.Button("Pick OLD folder"))
            {
                _oldReferences = EditorUtility.OpenFolderPanel("Pick folder with OLD GUIDs", _oldReferences, "");
            }
            
            GUILayout.Label("NEW folder: " + PrintFolderIsSet(_newReferences));
            if (GUILayout.Button("Pick NEW folder"))
            {
                _newReferences = EditorUtility.OpenFolderPanel("Pick folder with NEW GUIDs", _newReferences, "");
            }
            
            GUILayout.Label("TARGET folder: " + PrintFolderIsSet(_targetFolder));
            if (GUILayout.Button("Pick TARGET folder"))
            {
                _targetFolder = EditorUtility.OpenFolderPanel("Pick target folder to replace OLD GUIDs with NEW", _targetFolder, "");
            }

            if (GUILayout.Button("Parse"))
            {
                _parseResult = _guidRemappingTool.Parse(_oldReferences, _newReferences);
            }

            if (_parseResult == null)
            {
                return;
            }
            var uniqueOldFilenames = new Dictionary<string, GuidRemappingTool.Ref>();
            var uniqueNewFilenames = new Dictionary<string, GuidRemappingTool.Ref>();
            
            foreach(var item in _parseResult.OldRefs)
            {
                if (uniqueOldFilenames.ContainsKey(item.Filename))
                {
                    Debug.LogError("DUPLICATED NAME:  + item.Filename");
                    continue;
                }
                uniqueOldFilenames.Add(item.Filename, item);
            }
                    
            foreach(var item in _parseResult.NewRefs)
            {
                if (uniqueNewFilenames.ContainsKey(item.Filename))
                {
                    Debug.LogError("DUPLICATED NAME:  + item.Filename");
                    continue;
                }
                uniqueNewFilenames.Add(item.Filename, item);
            }
            
            GUILayout.Label($"OLD names: {_parseResult.OldRefs.Count}, All unique: {_parseResult.OldRefs.Count == uniqueOldFilenames.Count}");
            GUILayout.Label($"NEW names: {_parseResult.NewRefs.Count}, All unique: {_parseResult.NewRefs.Count == uniqueNewFilenames.Count}");

            using (var scrollView = new GUILayout.ScrollViewScope(_scrollPos))
            {
                _scrollPos = scrollView.scrollPosition;
                
                foreach (var oldRef in _parseResult.OldRefs)
                {
                    if (!uniqueNewFilenames.TryGetValue(oldRef.Filename, out var newRef))
                    {
                        continue;
                    }
                    
                    GUILayout.Label($"{newRef.Filename} OLD GUID: {oldRef.Guid}, NEW GUID: {newRef.Guid}");
                }
            }
            
            if (GUILayout.Button("Replace"))
            {
                var targetFiles = Directory.GetFiles(_targetFolder, "*",SearchOption.AllDirectories);
                foreach (var targetFilePath in targetFiles)
                {
                    var content = File.ReadAllText(targetFilePath);
                    var original = string.Copy(content);
                    var lastIteration = string.Copy(content);
                    foreach (var oldRef in _parseResult.OldRefs)
                    {
                        if (!uniqueNewFilenames.TryGetValue(oldRef.Filename, out var newRef))
                        {
                            continue;
                        }

                        content = content.Replace(oldRef.Guid, newRef.Guid);
                        if (lastIteration != content)
                        {
                            Debug.Log($"Replaced `{oldRef.Guid}` with: {newRef.Guid}, ref filename: `{newRef.Filename}` in file: `{targetFilePath}`");
                        }

                        lastIteration = string.Copy(content);

                    }

                    if (original != content)
                    {
                        Debug.Log("------------ Write file: " + targetFilePath);
                        File.WriteAllText(targetFilePath, content);
                    }
                }
            }
        }
        
        private readonly GuidRemappingTool _guidRemappingTool = new GuidRemappingTool();
        
        private string _oldReferences;
        private string _newReferences;
        private string _targetFolder;
        private GuidRemappingTool.ParseResult _parseResult;
        private Vector2 _scrollPos;
        
        private string PrintFolderIsSet(string source) => source.IsNullOrEmpty() ? "Undefined" : source;
    }

    public class GuidRemappingTool
    {
        public struct Ref
        {
            public readonly string Path;
            public readonly string Guid;
            public readonly string Filename;

            public Ref(string path, string guid, string filename)
            {
                Path = path;
                Guid = guid;
                Filename = filename;
            }

            public override string ToString() => $"{Path}::{Guid}::{Filename}";
        }
        public class ParseResult
        {
            public IReadOnlyList<Ref> OldRefs => _oldRefs;
            public IReadOnlyList<Ref> NewRefs => _newRefs;

            private readonly List<Ref> _oldRefs = new List<Ref>();
            private readonly List<Ref> _newRefs = new List<Ref>();

            public ParseResult(List<Ref> oldRefs, List<Ref> newRefs)
            {
                _oldRefs = oldRefs;
                _newRefs = newRefs;
            }
        }
        
        public ParseResult Parse(string oldFolder, string newFolder)
        {
            var oldRefs = new List<Ref>();
            var newRefs = new List<Ref>();
            var result = new ParseResult(oldRefs, newRefs);

            ParseDirectory(oldFolder, oldRefs);
            ParseDirectory(newFolder, newRefs);

            return result;
        }

        private static void ParseDirectory(string dirPath, ICollection<Ref> result)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }
            var oldFiles = Directory.GetFiles(dirPath, "*",SearchOption.AllDirectories);
            foreach (var filePath in oldFiles)
            {
                if (filePath.EndsWith(".meta"))
                {
                    continue;
                }
                
                if (filePath.EndsWith(".svg"))
                {
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    continue;
                }

                var filename = Path.GetFileName(filePath);
                var relativePath = "Assets" + filePath.Substring(Application.dataPath.Length);
                var guid = AssetDatabase.AssetPathToGUID(relativePath);

                if (guid.IsNullOrEmpty())
                {
                    Debug.LogWarning("Failed to get GUID from path: " + relativePath);
                    continue;
                }
                
                result.Add(new Ref(relativePath, guid, filename));
            }
        }
    }
}