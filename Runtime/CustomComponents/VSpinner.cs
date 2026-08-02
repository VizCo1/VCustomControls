using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    [UxmlElement]
    public partial class VSpinner : VisualElement, INotifyValueChanged<bool>
    {
        public static readonly string VSpinnerClass = "spinner";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly BindingId SpeedProperty = (BindingId) nameof(Speed);
        private static readonly BindingId RotationRateProperty = (BindingId) nameof(RotationRate);

        [Header(nameof(VSpinner))]
        
        [UxmlAttribute, CreateProperty]
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
        
        [UxmlAttribute, CreateProperty]
        public float Speed
        {
            get => _speed;
            set
            {
                if (Mathf.Approximately(value, _speed))
                    return;
                
                _speed = value;
                
                NotifyPropertyChanged(in SpeedProperty);
            }
        }

        [UxmlAttribute, CreateProperty]
        private long RotationRate
        {
            get => _rotationRate;
            set
            {
                _rotationRate = value;
                
                if (_rotationRate == value)
                    return;
                
                SetValueWithoutNotify(_value);
                
                NotifyPropertyChanged(in RotationRateProperty);
            }
        }

        private bool _value;
        private float _degrees;
        private float _speed = 5f;
        private long _rotationRate = 10;
        
        private IVisualElementScheduledItem _scheduledItem;

        public VSpinner()
        {
            AddToClassList(VSpinnerClass);
            usageHints = UsageHints.DynamicTransform;
        }
        
        public void ResetRotation()
        {
            _degrees = 0;
            style.rotate = new Rotate(new Angle(_degrees, AngleUnit.Degree));
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            _value = newValue;

            _scheduledItem?.Pause();

            if (!_value) 
                return;
            
            _scheduledItem = schedule
                .Execute(StartRotate)
                .Every(_rotationRate)
                .Until(() => !_value);
        }

        private void StartRotate(TimerState timerState)
        {
            _degrees += Speed;
            
            if (float.IsInfinity(_degrees))
            {
                _degrees = 0;
            }
            
            style.rotate = new Rotate(new Angle(_degrees, AngleUnit.Degree));
        }
    }
}