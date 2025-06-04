using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class RadialSliderView : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _exampleElement;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _exampleElement = _document.rootVisualElement.Q<VisualElement>();
        }

        private void OnDestroy() 
        {
        
        }
    }
}
