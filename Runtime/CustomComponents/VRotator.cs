using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    [UxmlElement]
    public partial class VRotator : VisualElement, INotifyValueChanged<int>
    {
        public static readonly string VRotatorClass = "rotator";
        public static readonly string TextName = "RotatorLabel";
        public static readonly string LeftButtonName = "LeftRotatorButton";
        public static readonly string RightButtonName = "RightRotatorButton";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        [Header(nameof(VRotator))]
        [UxmlAttribute, CreateProperty]
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
            if (this.TryGetVisualElement<TemplateContainer>(TextName, null, out var labelTemplate))
            {
                _label = labelTemplate.Q<Label>();
            }
            else if (_label == null && !this.TryGetVisualElement(TextName, null, out _label))
            {
                Debug.LogError($"No label or template container named '{TextName}' added to the Rotator");
                return;
            }

            SetValueWithoutNotify(value);
            
            if (!HasButtons)
                return;

            if (this.TryGetVisualElement(LeftButtonName, null, out TemplateContainer leftButtonTemplate))
            {
                _leftButton = leftButtonTemplate.Q<Button>();
            }
            else if (_leftButton == null && !this.TryGetVisualElement(LeftButtonName, null, out _leftButton))
            {
                Debug.LogError($"No button or template container named '{LeftButtonName}' added to the Rotator");
                return;
            }
            
            if (this.TryGetVisualElement(RightButtonName, null, out TemplateContainer rightButtonTemplate))
            {
                _rightButton = rightButtonTemplate.Q<Button>();
            }
            else if (_rightButton == null && !this.TryGetVisualElement(RightButtonName, null, out _rightButton))
            {
                Debug.LogError($"No button template container named '{RightButtonName}' added to the Rotator");
                return;
            }

            _leftButton.clicked += OnLeftButtonClicked;
            _rightButton.clicked += OnRightButtonClicked;
            
            _leftButton.SetEnabled(!(!AreButtonsLoopable && _value == 0));
            _rightButton.SetEnabled(!(!AreButtonsLoopable && _value == Options.Length - 1));
        }
        
        public void SetValueWithoutNotify(int newValue)
        {
            _value = Mathf.Clamp(newValue, 0, Options.Length - 1);
            
            if (_label == null)
                return;
            
            _label.text = Options[_value];
            
            _leftButton?.SetEnabled(!(!AreButtonsLoopable && _value == 0));
            _rightButton?.SetEnabled(!(!AreButtonsLoopable && _value == Options.Length - 1));
        }

        public string GetCurrentOption()
        {
            return _options[_value];
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
    }
}