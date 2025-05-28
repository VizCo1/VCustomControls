using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class SpinnerView : MonoBehaviour
    {
        private const string ButtonContainer1Name = "ExamplesButtonContainer1";
        private const string ButtonContainer2Name = "ExamplesButtonContainer2";
        
        private UIDocument _document;
        private VSpinner _spinner;
        private Button _buttonToggle;
        private Button _buttonReset;
        private Slider _slider;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _spinner = _document.rootVisualElement.Q<VSpinner>();
            _slider = _document.rootVisualElement.Q<Slider>();
            _buttonToggle = (Button)_document.rootVisualElement.Q(ButtonContainer1Name)[0];
            _buttonReset = (Button)_document.rootVisualElement.Q(ButtonContainer2Name)[0];
            
            _spinner.RegisterValueChangedCallback(OnSpinnerValueChanged);
            _slider.RegisterValueChangedCallback(OnSliderValueChanged);
            _buttonToggle.clicked += OnButtonToggleClicked;
            _buttonReset.clicked += OnButtonResetClicked;
        }

        private void OnDestroy()
        {
            _spinner.UnregisterValueChangedCallback(OnSpinnerValueChanged);
            _slider.UnregisterValueChangedCallback(OnSliderValueChanged);
            _buttonToggle.clicked -= OnButtonToggleClicked;
            _buttonReset.clicked -= OnButtonResetClicked;
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
