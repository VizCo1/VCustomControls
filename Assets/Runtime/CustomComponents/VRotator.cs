using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRotator : VisualElement, INotifyValueChanged<int>
    {
        private const string TextName = "RotatorLabel";
        private const string LeftButtonName = "LeftRotatorButton";
        private const string RightButtonName = "RightRotatorButton";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        public static readonly string VRotatorClass = "rotator"; 
        public static readonly string LeftButtonClass = VRotatorClass + "-left-button"; 
        public static readonly string RightButtonClass = VRotatorClass + "-right-button";
        public static readonly string TextClass = VRotatorClass + "-label";

        [Header(nameof(VRotator))]
        [UxmlAttribute]
        public int value
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;
                
                var previousValue = _value;
                SetValueWithoutNotify(value);
            
                if (panel == null) 
                    return;

                using var pooled = ChangeEvent<int>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }
        
        [UxmlAttribute]
        private bool HasButtons { get; set; }
        
        [UxmlAttribute]
        private bool AreButtonsLoopable { get; set; }

        [UxmlAttribute]
        public string[] Options
        {
            get => _options;
            set
            {
                if (value == null)
                    return;
                
                _options = value;

                this.value = 0;
            }
        }


        private Button _leftButton;
        private Button _rightButton;
        private Label _label;

        private int _value;
        private string[] _options = { "Default" };
        
        public VRotator() 
        {
            AddToClassList(VRotatorClass);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (!this.TryGetVisualElement(TextName, null, out _label))
            {
                Debug.LogError($"No label named '{TextName}' added to the Rotator");
                return;
            }

            _label.AddToClassList(TextClass);
            SetValueWithoutNotify(value);
            
            if (!HasButtons)
                return;

            if (this.TryGetVisualElement<TemplateContainer>(LeftButtonName, null, out var leftButtonTemplate))
            {
                _leftButton = leftButtonTemplate.Q<Button>();
            }
            
            if (this.TryGetVisualElement<TemplateContainer>(LeftButtonName, null, out var rightButtonTemplate))
            {
                _rightButton = rightButtonTemplate.Q<Button>();
            }
            
            if (_leftButton == null && !this.TryGetVisualElement(LeftButtonName, null, out _leftButton))
            {
                Debug.LogError($"No button named '{LeftButtonName}' attached to the Rotator");
                return;
            }
            
            if (_rightButton == null && !this.TryGetVisualElement(RightButtonName, null, out _rightButton))
            {
                Debug.LogError($"No button named '{RightButtonName}' attached to the Rotator");
                return;
            }
            
            _leftButton.AddToClassList(LeftButtonClass);
            _rightButton.AddToClassList(RightButtonClass);

            _leftButton.clicked += OnLeftButtonClicked;
            _rightButton.clicked += OnRightButtonClicked;
        }

        private void OnLeftButtonClicked()
        {
            if (AreButtonsLoopable && value == 0)
            {
                value = Options.Length - 1;
                return;
            }
            
            value--;
        }
        
        private void OnRightButtonClicked()
        {
            if (AreButtonsLoopable && value == Options.Length - 1)
            {
                value = 0;
                return;
            }
            
            value++;
        }

        public void SetValueWithoutNotify(int newValue)
        {
            _value = newValue.Clamp(0, Options.Length - 1);
            
            _label.text = Options[_value];
        }
    }
}