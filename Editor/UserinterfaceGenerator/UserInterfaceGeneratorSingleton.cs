using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterfaceGenerator.Editor
{
    [FilePath("UserInterfaceGenerator/UserInterfaceGeneratorEditorWindow.foo", FilePathAttribute.Location.ProjectFolder)]
    public class UserInterfaceGeneratorSingleton : ScriptableSingleton<UserInterfaceGeneratorSingleton>
    {
        [SerializeField]
        public string generatedFilesFolderPath;
        
        [SerializeField]
        public int generationType;
        
        [SerializeField]
        public VisualTreeAsset fileToGenerate;
    }
}