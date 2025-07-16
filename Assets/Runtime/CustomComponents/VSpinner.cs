using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VSpinner : VisualElement, INotifyValueChanged<bool>
    {
        public static readonly string VSpinnerClass = "spinner";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly BindingId SpeedProperty = (BindingId) nameof(Speed);

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
        
        [UxmlAttribute]
        private long RotationRate { get; set; } = 10;

        private bool _value;
        private float _degrees;
        private float _speed = 5f;
        
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
                .Every(RotationRate)
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