using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class AspectRatio : ViewBase
    {
        private const string VerticalSliderName = "VerticalSlider";
        private const string HorizontalSliderName = "HorizontalSlider";
        
        private VAspectRatio _aspectRatio;

        protected override void Start()
        {
            base.Start();
            
            _aspectRatio =  Root.Q<VAspectRatio>();
            var verticalSlider = (SliderInt)Root.Q(VerticalSliderName)[0];
            var horizontalSlider = (SliderInt)Root.Q(HorizontalSliderName)[0];

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
