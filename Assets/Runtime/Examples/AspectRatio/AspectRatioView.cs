using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class AspectRatioView : MonoBehaviour
    {
        private const string VerticalSliderName = "VerticalSlider";
        private const string HorizontalSliderName = "HorizontalSlider";
        
        private UIDocument _document;
        private VAspectRatio _aspectRatio;
        private SliderInt _verticalSlider;
        private SliderInt _horizontalSlider;
        
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _aspectRatio =  _document.rootVisualElement.Q<VAspectRatio>();
            _verticalSlider = (SliderInt)_document.rootVisualElement.Q(VerticalSliderName)[0];
            _horizontalSlider = (SliderInt)_document.rootVisualElement.Q(HorizontalSliderName)[0];

            _verticalSlider.value = _aspectRatio.RatioHeight;
            _horizontalSlider.value = _aspectRatio.RatioWidth;
            
            _verticalSlider.RegisterValueChangedCallback(OnVerticalSliderValueChanged);
            _horizontalSlider.RegisterValueChangedCallback(OnHorizontalSliderValueChanged);
        }
        
        private void OnVerticalSliderValueChanged(ChangeEvent<int> evt)
        {
            _aspectRatio.RatioHeight = evt.newValue;
        }

        private void OnHorizontalSliderValueChanged(ChangeEvent<int> evt)
        {
            _aspectRatio.RatioWidth = evt.newValue;
        }
    }
}
