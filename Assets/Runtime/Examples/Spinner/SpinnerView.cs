using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class SpinnerView : BaseView
    {
        private const string ButtonContainer1Name = "ExamplesButtonContainer1";
        private const string ButtonContainer2Name = "ExamplesButtonContainer2";
        
        private VSpinner _spinner;
        private Button _buttonToggle;
        private Button _buttonReset;
        private Slider _slider;

        protected override void Start()
        {
            base.Start();
            
            _spinner = _document.rootVisualElement.Q<VSpinner>();
            _slider = _document.rootVisualElement.Q<Slider>();
            _buttonToggle = (Button)_document.rootVisualElement.Q(ButtonContainer1Name)[0];
            _buttonReset = (Button)_document.rootVisualElement.Q(ButtonContainer2Name)[0];

            _slider.value = _spinner.Speed;
            
            _spinner.RegisterValueChangedCallback(OnSpinnerValueChanged);
            _slider.RegisterValueChangedCallback(OnSliderValueChanged);
            _buttonToggle.clicked += OnButtonToggleClicked;
            _buttonReset.clicked += OnButtonResetClicked;
        }

        protected override void OnDestroy()
        {
            _spinner.UnregisterValueChangedCallback(OnSpinnerValueChanged);
            _slider.UnregisterValueChangedCallback(OnSliderValueChanged);
            _buttonToggle.clicked -= OnButtonToggleClicked;
            _buttonReset.clicked -= OnButtonResetClicked;
            
            base.OnDestroy();
        }

        private void OnSpinnerValueChanged(ChangeEvent<bool> evt)
        {
            Debug.Log(evt.newValue);
        }
        
        private void OnSliderValueChanged(ChangeEvent<float> evt)
        {
            _spinner.Speed = evt.newValue;
        }
        
        private void OnButtonToggleClicked()
        {
            _spinner.value = !_spinner.value;
        }
        
        private void OnButtonResetClicked()
        {
            _spinner.ResetRotation();
        }
    }
}
