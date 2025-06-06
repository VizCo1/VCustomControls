using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRotator : VisualElement, INotifyValueChanged<int>
    {
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
        public string[] Options { get; set; }


        private Button _leftButton;
        private Button _rightButton;
        private Label _label;

        private int _value;
        
        public VRotator() 
        {
            AddToClassList(VRotatorClass);
            
            _leftButton = new Button();
            _leftButton.AddToClassList(LeftButtonClass);
            Add(_leftButton);

            _label = new Label();
            _label.AddToClassList(TextClass);
            Add(_label);
            
            _rightButton = new Button();
            _rightButton.AddToClassList(RightButtonClass);
            Add(_rightButton);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            // RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
        
        }
    
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
        
        }

        public void SetValueWithoutNotify(int newValue)
        {
            
        }
    }
}