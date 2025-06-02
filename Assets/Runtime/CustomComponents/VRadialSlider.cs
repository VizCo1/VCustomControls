using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialSlider : VisualElement, INotifyValueChanged<float>
    {
        private const string RadialSliderClass = "radial-slider";
        private const string RadialSliderDraggerClass = "radial-slider-dragger";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        private readonly VisualElement _draggerElement;
        
        [Header(nameof(VRadialSlider))]
        
        [UxmlAttribute]
        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
            
                SetValueWithoutNotify(this.value);
            }
        }

        [UxmlAttribute]
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
            
                SetValueWithoutNotify(this.value);
            }
        }
    
        [UxmlAttribute, CreateProperty]
        public float value
        {
            get => _value;
            set
            {
                if (Mathf.Approximately(value, _value))
                    return;
            
                var previousValue = _value;
                SetValueWithoutNotify(value);
            
                if (panel == null) 
                    return;

                using var pooled = ChangeEvent<float>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }

        [UxmlAttribute]
        public float StartingAngle
        {
            get => _startingAngle;
            set
            {
                _startingAngle = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public float EndingAngle
        {
            get => _endingAngle;
            set
            {
                _endingAngle = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        public float LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Vector2 CenterOffset
        {
            get => _centerOffset;
            set
            {
                _centerOffset = value;
                _center = contentRect.center + _centerOffset;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public LineCap LineCap
        {
            get => _lineCap;
            set
            {
                _lineCap = value;
                MarkDirtyRepaint();
            }
        }
        
        private float _value;
        private float _minValue;
        private float _maxValue = 1f;
        private float _startingAngle;
        private float _endingAngle = 360f;
        private Color _backgroundColor = Color.white;
        private Color _fillColor = new (0, 0, 0, 0);
        private Vector2 _centerOffset;
        private LineCap _lineCap = LineCap.Round;
        private float _lineWidth = 10f;

        private Vector2 _center;
        private bool _canMove;

        public VRadialSlider() 
        {
            AddToClassList(RadialSliderClass);
            
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanelEvent);
            
            _draggerElement = new VisualElement();
            _draggerElement.AddToClassList(RadialSliderDraggerClass);
            _draggerElement.style.width = 25f;
            _draggerElement.style.height = 25f;
            _draggerElement.style.borderBottomLeftRadius = 25f;
            _draggerElement.style.borderBottomRightRadius = 25f;
            _draggerElement.style.borderTopLeftRadius = 25f;
            _draggerElement.style.borderTopRightRadius = 25f;
            _draggerElement.style.backgroundColor = Color.black;
            _draggerElement.style.position = Position.Absolute;
            
            Add(_draggerElement);
        }

        private void OnAttachedToPanelEvent(AttachToPanelEvent evt)
        {
            _center = contentRect.center + _centerOffset;
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;
            
            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = _backgroundColor;
            painter2D.lineCap = _lineCap;
            
            var radius = contentRect.width * 0.5f - _lineWidth * 0.5f;
            
            painter2D.BeginPath();
            painter2D.Arc(_center, radius, _startingAngle, _endingAngle);
            painter2D.Stroke();
            
            if (_fillColor.a == 0f)
                return;
            
            var angle = (_value - _minValue) / (_maxValue - _minValue) * (_endingAngle - _startingAngle) + _startingAngle;
            
            painter2D.strokeColor = _fillColor;
            
            painter2D.BeginPath();
            painter2D.Arc(_center, radius, _startingAngle, angle);
            painter2D.Stroke();
        }
        
        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;
            
            this.CapturePointer(0);

            _canMove = true;
            
            Debug.Log("Left Mouse Down");
        }
        
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_canMove)
                return;

            var position = GetCircularPosition2D(0, 0, _center);
            
            _draggerElement.style.translate = new Translate(position.x, position.y);
        }
        
        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            if (evt.button != 0)
                return;
            
            this.ReleasePointer(0);

            _canMove = false;
            
            Debug.Log("Left Mouse Up");
        }
        
        private Vector2 GetCircularPosition2D(float angleRadians, float radius, Vector2 center)
        {
            var x = center.x + radius * Mathf.Cos(angleRadians);
            var y = center.y + radius * Mathf.Sin(angleRadians);
            
            return new Vector2(x, y);
        }
        
        public void SetValueWithoutNotify(float newValue)
        {
            _value = newValue.Clamp(_minValue, _maxValue);
            MarkDirtyRepaint();
        }
    }
}