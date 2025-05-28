using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VSlider2D : VisualElement, INotifyValueChanged<Vector2>
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
    
        public static readonly string VSlider2DClass = "slider2d"; 
        public static readonly string DraggerClass = VSlider2DClass + "-dragger-element";
    
        public static readonly string DraggerName = "DraggerElement";
    
        [Header(nameof(VSlider2D))]
        [UxmlAttribute]
        public Vector2 MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
            
                SetValueWithoutNotify(this.value);
            }
        }

        [UxmlAttribute]
        public Vector2 MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
            
                SetValueWithoutNotify(this.value);
            }
        }
    
        [UxmlAttribute]
        [CreateProperty]
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
    
        private VisualElement _draggerElement;

        public VSlider2D()
        {
            AddToClassList(VSlider2DClass);
        
            CreateDraggerElement();

            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _offset = new Vector2(_draggerElement.resolvedStyle.width / 2f, _draggerElement.resolvedStyle.height / 2f);
        
            MoveDragger();
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;
        
            value = RemapBetweenMinAndHighValues(evt.localMousePosition);
        
            _canMove = true;
            this.CapturePointer(evt.button);
        }
    
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_canMove)
                return;
        
            value = RemapBetweenMinAndHighValues(evt.localMousePosition);
        }
    
        private void OnMouseUp(MouseUpEvent evt)
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
            
            _draggerElement.style.translate = new Translate(new Length(adjustedPosX, LengthUnit.Pixel), new Length(adjustedPosY, LengthUnit.Pixel));
        }
    
        private Vector2 RemapBetweenMinAndHighValues(Vector2 position)
        {
            var posX = _minValue.x + position.x / resolvedStyle.width * (_maxValue.x - _minValue.x);
            var posY = _minValue.y + position.y / resolvedStyle.height * (_maxValue.y - _minValue.y);
        
            return new Vector2(posX, posY);
        }
    
        private void CreateDraggerElement()
        {
            _draggerElement = new VisualElement();
            _draggerElement.AddToClassList(DraggerClass);
            _draggerElement.name = DraggerName;
            _draggerElement.usageHints = UsageHints.DynamicTransform;
            Add(_draggerElement);
        }

        public void SetValueWithoutNotify(Vector2 newValue)
        {
            var validX = Mathf.Clamp(newValue.x, _minValue.x, _maxValue.x);
            var validY = Mathf.Clamp(newValue.y, _minValue.y, _maxValue.y);
            
            _value = new Vector2(validX, validY);
        
            MoveDragger();
        }
    }
}