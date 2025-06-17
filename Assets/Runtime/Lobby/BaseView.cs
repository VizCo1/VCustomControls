using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public abstract class BaseView : MonoBehaviour
    {
        private const string BackButtonContainerName = "BackButtonContainer";
        
        public VisualElement Root { get; private set; }
        public UIDocument Document { get; private set; }
        
        protected Button _backButton;

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
