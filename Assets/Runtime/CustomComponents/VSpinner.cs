using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VSpinner : VisualElement, INotifyValueChanged<bool>
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly BindingId SpeedProperty = (BindingId) nameof(Speed);
        
        public static readonly string VSpinnerClass = "spinner";

        [Header(nameof(VSpinner))]
        
        [UxmlAttribute, CreateProperty, Range(0, 1), Tooltip("This value indicates the rotation speed.")]
        public float Speed
        {
            get => _speed;
            set
            {
                if (Mathf.Approximately(value, _speed) || value > 1)
                    return;
                
                _speed = value;
                
                NotifyPropertyChanged(in SpeedProperty);
            }
        }
        
        [UxmlAttribute, CreateProperty, Tooltip("This value indicates whether or not the element is spinning.")]
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
        private float _speed;
        private IVisualElementScheduledItem _scheduledItem;
        
        public VSpinner() 
        {
            AddToClassList(VSpinnerClass);
            usageHints = UsageHints.DynamicTransform;
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

        private void StartRotate(TimerState timerState)
        {
            _degrees += Speed * timerState.deltaTime;
            
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