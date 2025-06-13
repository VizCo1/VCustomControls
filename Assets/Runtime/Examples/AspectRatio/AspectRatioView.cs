using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class AspectRatioView : BaseView
    {
        private const string VerticalSliderName = "VerticalSlider";
        private const string HorizontalSliderName = "HorizontalSlider";
        
        private VAspectRatio _aspectRatio;

        private void Start()
        {
            _aspectRatio =  _document.rootVisualElement.Q<VAspectRatio>();
            var verticalSlider = (SliderInt)_document.rootVisualElement.Q(VerticalSliderName)[0];
            var horizontalSlider = (SliderInt)_document.rootVisualElement.Q(HorizontalSliderName)[0];

            verticalSlider.value = _aspectRatio.RatioHeight;
            horizontalSlider.value = _aspectRatio.RatioWidth;
            
            verticalSlider.RegisterValueChangedCallback(OnVerticalSliderValueChanged);
            horizontalSlider.RegisterValueChangedCallback(OnHorizontalSliderValueChanged);
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
