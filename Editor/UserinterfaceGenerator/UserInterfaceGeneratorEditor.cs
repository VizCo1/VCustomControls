using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor.UserinterfaceGenerator
{
    public static class UserInterfaceGeneratorEditor
    {
        private static readonly Regex UxmlElementRegex = new("<(?:ui:)?([\\w:\\.]+).*?name=\"(_[^\"]+)\"", RegexOptions.Compiled);
        private const string SourceFolder = "Assets";
        private const string DestinationFolder = "Assets/Editor/UserInterfaceGenerator/SourceGenerationFiles";
        private const string AnalyzerName = ".UserInterfaceGenerator";
        private const string ExtensionForAnalyzer = ".additionalfile";
        
        [MenuItem("Tools/Generate UI Source Files")]
        private static void GenerateFilesManually()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                
                Directory.CreateDirectory(DestinationFolder);
    
                var files = Directory.GetFiles(DestinationFolder);
                foreach (var file in files)
                {
                    AssetDatabase.DeleteAsset(file);
                }
                
                var uxmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset", new[] { SourceFolder });
                foreach (var guid in uxmlGuids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    
                    if (IsValid(path))
                    {
                        HandleFile(path);
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
            
            Debug.Log("UI Reference containers have been generated");
        }
        
        private static void HandleFile(string originalPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalPath);
            var newFileName = fileName + AnalyzerName + ExtensionForAnalyzer;
            var newPath = DestinationFolder + "/" + newFileName;
            
            File.Copy(originalPath, newPath, overwrite: true); 
        }

        private static bool IsValid(string path)
        {
            return File.Exists(path) && File.ReadLines(path).Any(line => UxmlElementRegex.IsMatch(line));
        }
    }
}