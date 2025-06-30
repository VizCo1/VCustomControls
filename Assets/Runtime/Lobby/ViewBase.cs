using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ViewBase : MonoBehaviour
    {
        private const string BackButtonContainerName = "BackButtonContainer";
        
        public VisualElement Root { get; private set; }

        private UIDocument Document { get; set; }
        
        private Button _backButton;
        protected virtual void Awake()
        {
            Document = GetComponent<UIDocument>();
            Root = Document.rootVisualElement;
        }

        protected virtual void Start()
        {
            _backButton = (Button)Root.Query(BackButtonContainerName).Last().ElementAt(0);
            _backButton.clicked += OnBackButtonClicked;
        }

        protected virtual void BeforeDestroy()
        {
            
        }
        
        protected virtual void OnDestroy()
        {
            _backButton.clicked -= OnBackButtonClicked; 
        }
        
        private void OnBackButtonClicked()
        {
            BeforeDestroy();
            
            _backButton.SetEnabled(false);
            UiManager.Instance.PopDocument();
        }
    }
}
