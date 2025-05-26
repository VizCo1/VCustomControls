using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime.Examples._2DSlider
{
    [RequireComponent(typeof(UIDocument))]
    public class Slider2DView : MonoBehaviour
    {
        private UIDocument _document;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var slider2D = _document.rootVisualElement.Q<VCustomComponents.Runtime.CustomComponents.VSlider2D>();

            slider2D.RegisterValueChangedCallback(OnSlider2DValueChanged);
        }

        private void OnSlider2DValueChanged(ChangeEvent<Vector2> evt)
        {
            Debug.Log(evt.newValue);
        }
    }
}
