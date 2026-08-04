using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;

namespace Samples
{
    public class Spinner : VBaseView<SpinnerElements>
    {
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.SpeedSliderContainer.ExamplesSlider.value = Elements.VSpinner.Speed;
            
            Elements.VSpinner.RegisterValueChangedCallback(OnSpinnerValueChanged);
            Elements.SpeedSliderContainer.ExamplesSlider.RegisterValueChangedCallback(OnSliderValueChanged);
            Elements.ToggleSpinnerButtonContainer.ExamplesButton.clicked += OnButtonToggleClicked;
            Elements.ResetRotationButtonContainer.ExamplesButton.clicked += OnButtonResetClicked;
        }
        
        protected void OnDestroy()
        {
            Elements.VSpinner.UnregisterValueChangedCallback(OnSpinnerValueChanged);
            Elements.SpeedSliderContainer.ExamplesSlider.UnregisterValueChangedCallback(OnSliderValueChanged);
            Elements.ToggleSpinnerButtonContainer.ExamplesButton.clicked -= OnButtonToggleClicked;
            Elements.ResetRotationButtonContainer.ExamplesButton.clicked -= OnButtonResetClicked;
        }
        
        private void OnSpinnerValueChanged(ChangeEvent<bool> evt)
        {
            Debug.Log(evt.newValue);
        }
        
        private void OnSliderValueChanged(ChangeEvent<float> evt)
        {
            Elements.VSpinner.Speed = evt.newValue;
        }
        
        private void OnButtonToggleClicked()
        {
            Elements.VSpinner.value = !Elements.VSpinner.value;
        }
        
        private void OnButtonResetClicked()
        {
            Elements.VSpinner.ResetRotation();
        }
    }
}
