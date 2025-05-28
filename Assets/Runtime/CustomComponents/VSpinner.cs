using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VSpinner : VisualElement, INotifyValueChanged<bool>
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        public static readonly string VSpinnerClass = "spinner"; 
        
        [Header(nameof(VSpinner))]
        
        [UxmlAttribute]
        [Range(0, 10)]
        public float Speed { get; set; }
        
        [UxmlAttribute]
        [CreateProperty]
        [Tooltip("This value indicates whether or not the element is spinning")]
        public bool value
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

                using var pooled = ChangeEvent<bool>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }

        private bool _value;
        private float _degrees;
        private IVisualElementScheduledItem _scheduledItem;
        
        public VSpinner() 
        {
            AddToClassList(VSpinnerClass);
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            _value = newValue;

            _scheduledItem?.Pause();

            if (!_value) 
                return;
            
            _scheduledItem = schedule
                .Execute(StartRotate)
                .Every(10)
                .Until(() => !_value);
        }

        private void StartRotate()
        {
            _degrees += Speed;
            
            if (float.IsInfinity(_degrees))
            {
                _degrees = 0;
            }
            
            style.rotate = new Rotate(new Angle(_degrees, AngleUnit.Degree));
        }

        public void ResetRotation()
        {
            _degrees = 0;
            style.rotate = new Rotate(new Angle(_degrees, AngleUnit.Degree));
        }
    }
}