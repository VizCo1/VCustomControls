using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VCustomComponents.Runtime;

namespace UserInterfaceGenerator.Editor
{
    public class UserInterfaceGeneratorEditorWindow : EditorWindow
    {
        private const string PendingLogKey = "UIGenerator_PendingLog";
        private const string GenerateFilesText = "Generate files";
        private const string GenerateFileText = "Generate file";
        
        private const string MultipleFilesSourceFolder = "Assets";
        
        private const string AnalyzerName = ".UserInterfaceGenerator";
        private const string ExtensionForAnalyzer = ".additionalfile";
        
        [SerializeField]
        private VisualTreeAsset VisualTreeAsset;

        private UserInterfaceGeneratorElements _elements;
        private UserInterfaceGeneratorSingleton _singleton;

        [MenuItem("Tools/UI Generator")]
        public static void ShowExample()
        {
            var wnd = GetWindow<UserInterfaceGeneratorEditorWindow>();
            wnd.titleContent = new GUIContent("UI Generator");
        }

        public void CreateGUI()
        {
            var uxmlRoot = VisualTreeAsset.Instantiate();
            _elements = new UserInterfaceGeneratorElements(uxmlRoot, VisualTreeAsset);
            rootVisualElement.Add(uxmlRoot);

            _singleton = UserInterfaceGeneratorSingleton.instance;
            
            _elements.GeneratedFilesFolderObjectField.value = AssetDatabase.LoadAssetAtPath<DefaultAsset>(_singleton.generatedFilesFolderPath);
            
            _elements.GenerationTypeRadioButtonGroup.value = _singleton.generationType;
            HandleGenerationType();
            
            _elements.FileToGenerateObjectField.value = _singleton.fileToGenerate;
            
            _elements.GeneratedFilesFolderObjectField.RegisterCallback<ChangeEvent<Object>>(OnGeneratedFilesFolderChanged);
            _elements.GenerationTypeRadioButtonGroup.RegisterCallback<ChangeEvent<int>>(OnGenerationTypeChanged);
            _elements.FileToGenerateObjectField.RegisterCallback<ChangeEvent<Object>>(OnFileToGenerateChanged);
            _elements.GenerateButton.clicked += GenerateButtonClicked;
        }

        private void HandleButtonEnabled()
        {
            var hasFolderSelected = _elements.GeneratedFilesFolderObjectField.value as DefaultAsset != null;
            var hasUxmlSelected = _elements.FileToGenerateObjectField.value != null;
            
            _elements.GenerateButton.SetEnabled(
                    hasFolderSelected && 
                    (hasUxmlSelected && _elements.GenerationTypeRadioButtonGroup.value == 1 || _elements.GenerationTypeRadioButtonGroup.value == 0));
        }

        private void HandleGenerationType()
        {
            if (_singleton.generationType == 1)
            {
                _elements.FileToGenerateObjectField.SetDisplay(true);
                _elements.GenerateButton.text = GenerateFileText;
            }
            else
            {
                _elements.FileToGenerateObjectField.SetDisplay(false);
                _elements.GenerateButton.text = GenerateFilesText;
            }
        }
        
        private void GenerateFiles()
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                switch (_singleton.generationType)
                {
                    // Multiple Uxml files
                    case 0:
                    {
                        var files = Directory.GetFiles(_singleton.generatedFilesFolderPath);
                        foreach (var path in files)
                        {
                            if (path.Contains(ExtensionForAnalyzer))
                            {
                                AssetDatabase.DeleteAsset(path);
                            }
                        }

                        var uxmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset", new[] { MultipleFilesSourceFolder });
                        foreach (var guid in uxmlGuids)
                        {
                            var path = AssetDatabase.GUIDToAssetPath(guid);
                            HandleFile(path);
                        }
                        
                        break;
                    }
                    // One Uxml file
                    case 1:
                    {
                        var files = Directory.GetFiles(_singleton.generatedFilesFolderPath);
                        foreach (var file in files)
                        {
                            var fileName = _singleton.fileToGenerate.name;
                            if (fileName == fileName + AnalyzerName)
                            {
                                AssetDatabase.DeleteAsset(file);
                            }
                        }

                        var path = AssetDatabase.GetAssetPath(_singleton.fileToGenerate);
                        HandleFile(path);
                        
                        break;
                    }
                }
            }
            finally
            {
                SessionState.SetBool("UIGenerator_PendingLog", true);
                
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }

        private void HandleFile(string originalPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(originalPath);
            var newFileName = fileName + AnalyzerName + ExtensionForAnalyzer;
            var newPath = _singleton.generatedFilesFolderPath + "/" + newFileName;
            
            File.Copy(originalPath, newPath, overwrite: true); 
        }

        private void OnFileToGenerateChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null)
            {
                Debug.LogWarning("You must select a UXML in: File to generate");
            }

            _singleton.fileToGenerate = (VisualTreeAsset)evt.newValue;
            HandleButtonEnabled();
        }
        
        private void OnGeneratedFilesFolderChanged(ChangeEvent<Object> evt)
        {
            HandleButtonEnabled();
            
            var defaultAsset = evt.newValue as DefaultAsset;
            if (!defaultAsset)
            {
                Debug.LogWarning("You must select a folder in: Generated files folder. This value has not been saved");
                return;
            }

            _singleton.generatedFilesFolderPath = AssetDatabase.GetAssetPath(defaultAsset);
        }

        private void OnGenerationTypeChanged(ChangeEvent<int> evt)
        {
            _singleton.generationType = evt.newValue;
            
            HandleGenerationType();
            HandleButtonEnabled();
        }
        
        private void GenerateButtonClicked()
        {
            GenerateFiles();
        }
        
        [InitializeOnLoadMethod]
        private static void OnPostAssemblyReload()
        {
            if (!SessionState.GetBool(PendingLogKey, true)) 
                return;
            
            SessionState.SetBool(PendingLogKey, false);
            Debug.Log("Elements classes have been generated.");
        }
    }
}