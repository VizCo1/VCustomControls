using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public abstract class BaseView : MonoBehaviour
    {
        private const string BackButtonContainerName = "BackButtonContainer";
        
        public VisualElement Root { get; private set; }
        
        protected Button _backButton;
        
        private UIDocument _document;

        protected virtual void Awake()
        {
            _document = GetComponent<UIDocument>();
            Root =  _document.rootVisualElement;
        }

        protected virtual void Start()
        {
            _backButton = (Button)Root.Query(BackButtonContainerName).Last().ElementAt(0);
            
            _backButton.clicked += OnBackButtonClicked;
        }

        private void OnBackButtonClicked()
        {
            _backButton.SetEnabled(false);
            ManagerUI.Instance.PopDocument();
        }

        protected virtual void OnDestroy()
        {
            _backButton.clicked -= OnBackButtonClicked; 
        }
    }
}
