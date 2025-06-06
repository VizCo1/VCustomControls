using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class Slider2DView : MonoBehaviour
    {
        private UIDocument _document;
        private VSlider2D _slider2D;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _slider2D = _document.rootVisualElement.Q<VSlider2D>();

            _slider2D.RegisterValueChangedCallback(OnSlider2DValueChanged);
        }
        
        private void OnDestroy()
        {
            _slider2D.UnregisterValueChangedCallback(OnSlider2DValueChanged);
        }

        private void OnSlider2DValueChanged(ChangeEvent<Vector2> evt)
        {
            Debug.Log(evt.newValue);
        }
    }
}
