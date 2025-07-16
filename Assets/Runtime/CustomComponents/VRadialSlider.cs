using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialSlider : VisualElement, IVHasCustomEvent, INotifyValueChanged<float>
    {
        public static readonly string VRadialSliderClass = "radial-slider";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly CustomStyleProperty<Color> RadialBackgroundColor = new("--radial-background-color");
        private static readonly CustomStyleProperty<Color> RadialFillColor = new("--radial-fill-color");
        private static readonly CustomStyleProperty<Color> RadialDraggerColor = new("--radial-dragger-color");
        private static readonly CustomStyleProperty<float> RadialBackgroundWidth = new("--radial-background-width");
        private static readonly CustomStyleProperty<float> RadialFillWidth = new("--radial-fill-width");
        private static readonly CustomStyleProperty<float> RadialDraggerWidth = new("--radial-dragger-width");
        
        private const float DegreesInCircle = 360f;
        
        private readonly VExtendedClickable _vExtendedClickable;

        [Header(nameof(VRadialSlider))]
        [UxmlAttribute]
        public bool IsLoopable { get; set; }
        
        [UxmlAttribute]
        public bool IsInteractive
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
        
        public VCustomEventType CustomEvent { get; }
        
        private bool _isInteractive;
        private float _value;
        private float _minValue;
        private float _maxValue = 1f;
        private float _startingAngle;
        private float _endingAngle = 360f;

        private Vector2 _centerOffset;
        private Color _radialBackgroundColor;
        private Color _radialFillColor;
        private Color _radialDraggerColor;
        private float _radialBackgroundWidth;
        private float _radialFillWidth;
        private float _radialDraggerWidth;
        
        private LineCap _backgroundLineCap = LineCap.Round;
        private LineCap _fillLineCap = LineCap.Round;
        private LineCap _draggerLineCap = LineCap.Round;
        private float _draggerOffset1 = 5f;
        private float _draggerOffset2 = 5f;
        
        private Vector2 _center;
        private float _previousAngle;
        private bool _canPointerMove;

        public VRadialSlider() 
        {
            AddToClassList(VRadialSliderClass);

            CustomEvent |= VCustomEventType.AimEvent;
            
            _vExtendedClickable = new VExtendedClickable();
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _center = contentRect.center + _centerOffset;
        }
    
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == CustomStyleResolvedEvent.TypeId())
            {
                OnStylesResolved((CustomStyleResolvedEvent)evt);
            }

            if (!_isInteractive)
                return;

            if (evt.eventTypeId == PointerMoveEvent.TypeId())
            {
                OnPointerMove((PointerMoveEvent)evt);
            }
            else if (evt.eventTypeId == PointerUpEvent.TypeId())
            {
                OnPointerUp();
            }
            else if (evt.eventTypeId == VAimEvent.TypeId())
            {
                OnAimed((VAimEvent)evt);
            }
        }

        private void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(RadialBackgroundColor, out var radialBackgroundColor)) 
            {
                _radialBackgroundColor = radialBackgroundColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialFillColor, out var radialFillColor)) 
            {
                _radialFillColor = radialFillColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialDraggerColor, out var radialDraggerColor)) 
            {
                _radialDraggerColor = radialDraggerColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialBackgroundWidth, out var radialBackgroundWidth)) 
            {
                _radialBackgroundWidth = radialBackgroundWidth;
            }
            
            if (evt.customStyle.TryGetValue(RadialFillWidth, out var radialFillWidth)) 
            {
                _radialFillWidth = radialFillWidth;
            }
            
            if (evt.customStyle.TryGetValue(RadialDraggerWidth, out var radialDraggerWidth)) 
            {
                _radialDraggerWidth = radialDraggerWidth;
            }
            
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;

            var radius = contentRect.width * 0.5f - _radialBackgroundWidth * 0.5f;
            var angle = (_value - _minValue) / (_maxValue - _minValue) * (_endingAngle - _startingAngle) + _startingAngle;
            
            if (_radialBackgroundColor.a != 0f)
            {
                painter2D.lineWidth = _radialBackgroundWidth;
                painter2D.strokeColor = _radialBackgroundColor;
                painter2D.lineCap = _backgroundLineCap;
                
                // Draw background
                painter2D.BeginPath();
                painter2D.Arc(_center, radius, _startingAngle, _endingAngle);
                painter2D.Stroke();
            }
            
            if (_radialFillColor.a != 0f)
            {
                painter2D.strokeColor = _radialFillColor;
                painter2D.lineWidth = _radialFillWidth;
                painter2D.lineCap = _fillLineCap;
                
                // Draw fill
                painter2D.BeginPath();
                painter2D.Arc(_center, radius, _startingAngle, angle);
                painter2D.Stroke();
            }

            if (_radialDraggerColor.a == 0f)
                return;
            
            var circlePathPos1 = VMathExtensions.GetCircumferencePoint(angle, radius - _draggerOffset1, _center);
            var circlePathPos2 = VMathExtensions.GetCircumferencePoint(angle, radius + _draggerOffset2, _center);
            
            painter2D.strokeColor = _radialDraggerColor;
            painter2D.lineWidth = _radialDraggerWidth;
            painter2D.lineCap = _draggerLineCap;
            
            // Draw dragger
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
            
            _canPointerMove = true;
            
            var dir = (Vector2)evt.localPosition - _center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += DegreesInCircle;
            }

            _previousAngle = angle;
            
            value = (_maxValue - _minValue) * (angle - _startingAngle) / (_endingAngle - _startingAngle) + _minValue;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_canPointerMove)
                return;
            
            var dir = (Vector2)evt.localPosition - _center;
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
        
        private void OnAimed(VAimEvent evt)
        {
            if (evt.Aim == Vector2.zero)
                return;
            
            var angle = Mathf.Atan2(evt.Aim.y, evt.Aim.x) * Mathf.Rad2Deg;
            
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
        
        private void OnPointerUp()
        {
            _canPointerMove = false;            
            this.ReleasePointer(0);
        }
        
        public void SetValueWithoutNotify(float newValue)
        {
            _value = Mathf.Clamp(newValue, _minValue, _maxValue);
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