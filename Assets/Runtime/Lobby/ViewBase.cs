using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ViewBase : MonoBehaviour
    {
        private const string BackButtonContainerName = "BackButtonContainer";
        
        public VisualElement Root { get; private set; }
        
        protected Button BackButton { get; private set; }
        
        private UIDocument Document { get; set; }
        
        protected virtual void Awake()
        {
            Document = GetComponent<UIDocument>();
            Root = Document.rootVisualElement;
        }

        protected virtual void Start()
        {
            BackButton = (Button)Root.Query(BackButtonContainerName).Last().ElementAt(0);
            BackButton.clicked += OnBackButtonClicked;
        }

        private void OnBackButtonClicked()
        {
            BackButton.SetEnabled(false);
            UiManager.Instance.PopDocument();
        }

        protected virtual void OnDestroy()
        {
            BackButton.clicked -= OnBackButtonClicked; 
        }
    }
}
