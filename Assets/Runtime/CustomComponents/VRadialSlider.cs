using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialSlider : VisualElement, INotifyValueChanged<float>
    {
        public static readonly string RadialSliderClass = "radial-slider";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly CustomStyleProperty<Color> RadialBackgroundColor = new("--radial-background-color");
        private static readonly CustomStyleProperty<Color> RadialFillColor = new("--radial-fill-color");
        private static readonly CustomStyleProperty<Color> RadialDraggerColor = new("--radial-dragger-color");
        
        private const float DegreesInCircle = 360f;
        
        private readonly VExtendedClickable _vExtendedClickable;

        [Header(nameof(VRadialSlider))]
        [UxmlAttribute]
        private bool IsLoopable { get; set; }
        
        [UxmlAttribute]
        private bool IsInteractive
        {
            get => _isInteractive;
            set
            {
                _isInteractive = value;

                if (_isInteractive)
                {
                    this.AddManipulator(_vExtendedClickable);
                    _vExtendedClickable.PointerDown += OnPointerDown;
                }
                else
                {
                    this.RemoveManipulator(_vExtendedClickable);
                    _vExtendedClickable.PointerDown -= OnPointerDown;
                }
            }
        }
        
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

        [UxmlAttribute, Range(0f, 360f)]
        public float StartingAngle
        {
            get => _startingAngle;
            set
            {
                _startingAngle = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute, Range(0f, 360f)]
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

        [Header("Background")]
        
        [UxmlAttribute]
        public float BackgroundLineWidth
        {
            get => _backgroundLineWidth;
            set
            {
                _backgroundLineWidth = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public LineCap BackgroundLineCap
        {
            get => _backgroundLineCap;
            set
            {
                _backgroundLineCap = value;
                MarkDirtyRepaint();
            }
        }
        
        [Header("Fill")]
        
        [UxmlAttribute]
        public float FillLineWidth
        {
            get => _fillLineWidth;
            set
            {
                _fillLineWidth = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public LineCap FillLineCap
        {
            get => _fillLineCap;
            set
            {
                _fillLineCap = value;
                MarkDirtyRepaint();
            }
        }
        
        [Header("Dragger")]
        
        [UxmlAttribute]
        public float DraggerWidth
        {
            get => _draggerWidth;
            set
            {
                _draggerWidth = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public LineCap DraggerLineCap
        {
            get => _draggerLineCap;
            set
            {
                _draggerLineCap = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public float DraggerOffset1
        {
            get => _draggerOffset1;
            set
            {
                _draggerOffset1 = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public float DraggerOffset2
        {
            get => _draggerOffset2;
            set
            {
                _draggerOffset2 = value;
                MarkDirtyRepaint();
            }
        }

        private bool _isInteractive = true;
        
        private float _value;
        private float _minValue;
        private float _maxValue = 1f;
        private float _startingAngle;
        private float _endingAngle = 360f;

        private Vector2 _centerOffset;
        private Color _radialBackgroundColor;
        private Color _radialFillColor;
        private Color _radialDraggerColor;
        private LineCap _backgroundLineCap = LineCap.Round;
        private LineCap _fillLineCap = LineCap.Round;
        private LineCap _draggerLineCap = LineCap.Round;
        private float _backgroundLineWidth = 10f;
        private float _fillLineWidth = 10f;
        private float _draggerWidth = 10f;
        
        private float _draggerOffset1 = 5f;
        private float _draggerOffset2 = 5f;
        
        private Vector2 _center;
        private float _previousAngle;

        public VRadialSlider() 
        {
            AddToClassList(RadialSliderClass);

            _vExtendedClickable = new VExtendedClickable(OnClicked);
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _center = contentRect.center + _centerOffset;
        }
        
        private void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            evt.customStyle.TryGetValue(RadialBackgroundColor, out _radialBackgroundColor);
            evt.customStyle.TryGetValue(RadialFillColor, out _radialFillColor);
            evt.customStyle.TryGetValue(RadialDraggerColor, out _radialDraggerColor);

            if (_radialBackgroundColor == null || _radialFillColor == null || _radialDraggerColor == null)
                return;
            
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;

            var backgroundColor = _radialBackgroundColor;
            var fillColor = _radialFillColor;
            var draggerColor = _radialDraggerColor;
            
            painter2D.lineWidth = _backgroundLineWidth;
            painter2D.strokeColor = backgroundColor;
            painter2D.lineCap = _backgroundLineCap;
            
            var radius = contentRect.width * 0.5f - _backgroundLineWidth * 0.5f;
            
            painter2D.BeginPath();
            painter2D.Arc(_center, radius, _startingAngle, _endingAngle);
            painter2D.Stroke();
            
            var angle = (_value - _minValue) / (_maxValue - _minValue) * (_endingAngle - _startingAngle) + _startingAngle;

            if (fillColor.a != 0f)
            {
                painter2D.strokeColor = fillColor;
                painter2D.lineWidth = _fillLineWidth;
                painter2D.lineCap = _fillLineCap;
                
                painter2D.BeginPath();
                painter2D.Arc(_center, radius, _startingAngle, angle);
                painter2D.Stroke();
            }
            
            var circlePathPos1 = VMathExtensions.GetCircumferencePoint(angle, radius - _draggerOffset1, _center);
            var circlePathPos2 = VMathExtensions.GetCircumferencePoint(angle, radius + _draggerOffset2, _center);
            
            painter2D.strokeColor = draggerColor;
            painter2D.lineWidth = _draggerWidth;
            painter2D.lineCap = _draggerLineCap;
            
            painter2D.BeginPath();
            painter2D.MoveTo(circlePathPos1);
            painter2D.LineTo(circlePathPos2);
            painter2D.Stroke();
        }
        
        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;
            
            this.CapturePointer(0);
            
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            
            var dir = (Vector2)evt.localPosition - _center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += DegreesInCircle;
            }

            _previousAngle = angle;
            
            value = (_maxValue - _minValue) * (angle - _startingAngle) / (_endingAngle - _startingAngle) + _minValue;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            var dir = evt.localMousePosition - _center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            if (angle < 0)
            {
                angle += DegreesInCircle;
            }

            if (!IsLoopable)
            {
                if (Mathf.DeltaAngle(_previousAngle, angle) > 0f)
                {
                    if (_previousAngle > angle)
                    {
                        value = _maxValue;
                        return;
                    }
                }
                else
                {
                    if (_previousAngle < angle)
                    {
                        value = _minValue;
                        return;
                    }
                }
                
                _previousAngle = angle;
            }
            
            value = (_maxValue - _minValue) * (angle - _startingAngle) / (_endingAngle - _startingAngle) + _minValue;
        }
        
        private void OnClicked()
        {
            UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            this.ReleasePointer(0);
        }
        
        public void SetValueWithoutNotify(float newValue)
        {
            _value = newValue.Clamp(_minValue, _maxValue);
            MarkDirtyRepaint();
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            var distance = Vector2.Distance(localPoint, _center);
            var radius = contentRect.width * 0.5f;
            
            return distance < radius;
        }
    }
}