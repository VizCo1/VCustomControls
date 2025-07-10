using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class RadialSlider : ViewBase
    {
        private const string FocusButtonName = "FocusButton";
        
        private VRadialSlider _radialSlider;
        private Button _focusButton;
        
        protected override void Start()
        {
            base.Start();
            
            _radialSlider = Root.Q<VRadialSlider>();
            
            _focusButton = Root.Q<VisualElement>(FocusButtonName).Q<Button>();
            
            _focusButton.clicked += OnFocusButtonClicked;
        }

        private void OnFocusButtonClicked()
        {
            _radialSlider.ToggleFocus();
        }
        
        protected override void BeforeDestroy()
        {
            _focusButton.clicked -= OnFocusButtonClicked;
        }
    }
}
