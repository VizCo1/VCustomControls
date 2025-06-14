using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public abstract class BaseView : MonoBehaviour
    {
        private const string BackButtonContainerName = "BackButtonContainer";
        
        protected UIDocument _document;
        protected Button _backButton;

        protected virtual void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        protected virtual void Start()
        {
            _backButton = (Button)_document.rootVisualElement.Q(BackButtonContainerName).ElementAt(0);

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
