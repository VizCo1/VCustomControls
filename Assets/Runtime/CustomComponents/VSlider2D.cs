using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VSlider2D : VisualElement, INotifyValueChanged<Vector2>
    {
        public static readonly string VSlider2DClass = "slider2d"; 
        public static readonly string DraggerClass = VSlider2DClass + "-dragger-element";
        public static readonly string DraggerName = "DraggerElement";
    
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        private readonly VisualElement _draggerElement;
    
        [Header(nameof(VSlider2D))]
        
        [UxmlAttribute]
        public Vector2 MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
            
                SetValueWithoutNotify(_value);
            }
        }

        [UxmlAttribute]
        public Vector2 MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
            
                SetValueWithoutNotify(_value);
            }
        }
    
        [UxmlAttribute, CreateProperty]
        public Vector2 value
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

                using var pooled = ChangeEvent<Vector2>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }

        private bool _canMove;
        private Vector2 _offset;
    
        private Vector2 _value;
        private Vector2 _minValue = Vector2.zero;
        private Vector2 _maxValue = Vector2.one;
        
        public VSlider2D()
        {
            AddToClassList(VSlider2DClass);
        
            _draggerElement = new VisualElement();
            _draggerElement.AddToClassList(DraggerClass);
            _draggerElement.name = DraggerName;
            _draggerElement.usageHints = UsageHints.DynamicTransform;
            
            Add(_draggerElement);

            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _offset = new Vector2(_draggerElement.resolvedStyle.width * 0.5f, _draggerElement.resolvedStyle.height * 0.5f);
        
            MoveDragger();
        }
        
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == PointerMoveEvent.TypeId())
            {
                OnPointerMove((PointerMoveEvent)evt);
            }
            else if (evt.eventTypeId == PointerDownEvent.TypeId())
            {
                OnPointerDown((PointerDownEvent)evt);
            }
            else if (evt.eventTypeId == PointerUpEvent.TypeId())
            {
                OnPointerUp((PointerUpEvent)evt);
            }
        }
        
        public void SetValueWithoutNotify(Vector2 newValue)
        {
            var validX = Mathf.Clamp(newValue.x, _minValue.x, _maxValue.x);
            var validY = Mathf.Clamp(newValue.y, _minValue.y, _maxValue.y);
            
            _value = new Vector2(validX, validY);
        
            MoveDragger();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;
        
            value = RemapBetweenMinAndHighValues(evt.localPosition);
        
            _canMove = true;
            this.CapturePointer(evt.button);
        }
    
        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_canMove)
                return;
        
            value = RemapBetweenMinAndHighValues(evt.localPosition);
        }
    
        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button != 0)
                return;
        
            _canMove = false;
            this.ReleasePointer(evt.button);
        }
    
        private void MoveDragger()
        {
            var remappedPercentageX = (_value.x - _minValue.x) / (_maxValue.x - _minValue.x);
            var remappedPercentageY = (_value.y - _minValue.y) / (_maxValue.y - _minValue.y);
        
            var adjustedPosX = remappedPercentageX * resolvedStyle.width - _offset.x - resolvedStyle.paddingLeft - resolvedStyle.borderLeftWidth;
            var adjustedPosY = remappedPercentageY * resolvedStyle.height - _offset.y - resolvedStyle.paddingTop - resolvedStyle.borderTopWidth;
            
            adjustedPosX = Mathf.Clamp(
                adjustedPosX, 
                resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth, 
                resolvedStyle.width - _draggerElement.resolvedStyle.width 
                                    - resolvedStyle.borderRightWidth - resolvedStyle.borderLeftWidth);
            
            adjustedPosY = Mathf.Clamp(
                adjustedPosY, 
                resolvedStyle.borderTopWidth - resolvedStyle.borderBottomWidth, 
                resolvedStyle.height - _draggerElement.resolvedStyle.height 
                                     - resolvedStyle.borderBottomWidth - resolvedStyle.borderTopWidth);
            
            _draggerElement.style.translate = new Translate(new Length(adjustedPosX, LengthUnit.Pixel), new Length(adjustedPosY, LengthUnit.Pixel));
        }
    
        private Vector2 RemapBetweenMinAndHighValues(Vector2 position)
        {
            var posX = _minValue.x + position.x / resolvedStyle.width * (_maxValue.x - _minValue.x);
            var posY = _minValue.y + position.y / resolvedStyle.height * (_maxValue.y - _minValue.y);
        
            return new Vector2(posX, posY);
        }
    }
}