using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class LobbyView : BaseView
    {
        [SerializeField]
        private int _numberOfColumns = 2;
        
        [SerializeField]
        private int _numberOfExamples;
        
        private VGridListView _libraryGridListView;

        private void Start()
        {
            var root = _document.rootVisualElement;
            
            _libraryGridListView = _document.rootVisualElement.Q<VGridListView>();
        }

        private void OnDestroy() 
        {
        
        }
    }
}
