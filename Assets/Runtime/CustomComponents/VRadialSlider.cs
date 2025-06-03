using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialSlider : VisualElement, INotifyValueChanged<float>
    {
        private const string RadialSliderClass = "radial-slider";
        private const float DegreesInCircle = 360f;
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);

        [Header(nameof(VRadialSlider))]
        [UxmlAttribute]
        private bool IsInteractive
        {
            get => _isInteractive;
            set
            {
                _isInteractive = value;

                if (_isInteractive)
                {
                    RegisterCallback<MouseDownEvent>(OnMouseDown);
                    RegisterCallback<MouseMoveEvent>(OnMouseMove);
                    RegisterCallback<MouseUpEvent>(OnMouseUp);
                    RegisterCallback<MouseEnterEvent>(OnMouseEnter);
                    RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
                }
                else
                {
                    UnregisterCallback<MouseDownEvent>(OnMouseDown);
                    UnregisterCallback<MouseMoveEvent>(OnMouseMove);
                    UnregisterCallback<MouseUpEvent>(OnMouseUp);
                    UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
                    UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
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
        public Color BackgroundColorHover
        {
            get => _backgroundColorHover;
            set
            {
                _backgroundColorHover = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Color BackgroundColorActive
        {
            get => _backgroundColorActive;
            set
            {
                _backgroundColorActive = value;
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
        public Color FillColorHover
        {
            get => _fillColorHover;
            set
            {
                _fillColorHover = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Color FillColorActive
        {
            get => _fillColorActive;
            set
            {
                _fillColorActive = value;
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
        
        [UxmlAttribute]
        public Color DraggerColor
        {
            get => _draggerColor;
            set
            {
                _draggerColor = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Color DraggerColorHover
        {
            get => _draggerColorHover;
            set
            {
                _draggerColorHover = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        public Color DraggerColorActive
        {
            get => _draggerColorActive;
            set
            {
                _draggerColorActive = value;
                MarkDirtyRepaint();
            }
        }
        
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

        private Color _backgroundColor = new(200, 200, 200);
        private Color _backgroundColorHover = new(225, 225, 225);
        private Color _backgroundColorActive = new(255, 255, 255);
        private Color _fillColor = new (0, 0, 0, 0);
        private Color _fillColorHover = new (0, 0, 0, 0);
        private Color _fillColorActive = new (0, 0, 0, 0);
        private Vector2 _centerOffset;
        private LineCap _lineCap = LineCap.Round;
        private float _lineWidth = 10f;
        
        private Color _draggerColor = new(80, 80, 80);
        private Color _draggerColorHover = new(55, 55, 55);
        private Color _draggerColorActive = new(35, 35, 35);
        private float _draggerWidth = 10f;
        private float _draggerOffset1 = 5f;
        private float _draggerOffset2 = 5f;
        
        private Vector2 _center;
        private bool _canMove;
        private State _state;

        public VRadialSlider() 
        {
            AddToClassList(RadialSliderClass);

            _state = State.Normal;
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            _center = contentRect.center + _centerOffset;
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;

            var backgroundColor = _backgroundColor;
            var fillColor = _fillColor;
            var draggerColor = _draggerColor;

            if (_state.HasFlag(State.Active))
            {
                backgroundColor = _backgroundColorActive;
                fillColor = _fillColorActive;
                draggerColor = _draggerColorActive;
            }
            else if (_state.HasFlag(State.Hover))
            {
                backgroundColor = _backgroundColorHover;
                fillColor = _fillColorHover;
                draggerColor = _draggerColorHover;
            }
            
            painter2D.lineWidth = _lineWidth;
            painter2D.strokeColor = backgroundColor;
            painter2D.lineCap = _lineCap;
            
            var radius = contentRect.width * 0.5f - _lineWidth * 0.5f;
            
            painter2D.BeginPath();
            painter2D.Arc(_center, radius, _startingAngle, _endingAngle);
            painter2D.Stroke();
            
            var angle = (_value - _minValue) / (_maxValue - _minValue) * (_endingAngle - _startingAngle) + _startingAngle;

            if (fillColor.a != 0f)
            {
                painter2D.strokeColor = fillColor;
                
                painter2D.BeginPath();
                painter2D.Arc(_center, radius, _startingAngle, angle);
                painter2D.Stroke();
            }
            
            var circlePathPos1 = GetCircularPosition2D(angle, radius - _draggerOffset1, _center);
            var circlePathPos2 = GetCircularPosition2D(angle, radius + _draggerOffset2, _center);
            
            painter2D.strokeColor = draggerColor;
            painter2D.lineWidth = _draggerWidth;
            
            painter2D.BeginPath();
            painter2D.MoveTo(circlePathPos1);
            painter2D.LineTo(circlePathPos2);
            painter2D.Stroke();
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _state |= State.Hover;
            MarkDirtyRepaint();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            _state &= ~State.Hover;
            MarkDirtyRepaint();
        }
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;
            
            this.CapturePointer(0);
            
            _state |= State.Active;

            var dir = evt.localMousePosition - _center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += DegreesInCircle;
            }
            
            value = (_maxValue - _minValue) * (angle - _startingAngle) / (_endingAngle - _startingAngle) + _minValue;
            
            _canMove = true;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!_canMove)
                return;

            var dir = evt.localMousePosition - _center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += DegreesInCircle;
            }

            var arcLength = (_endingAngle - _startingAngle + DegreesInCircle) % DegreesInCircle;
            var relativeAngle = (angle - _startingAngle + DegreesInCircle) % DegreesInCircle;

            if (relativeAngle > arcLength)
                return;

            value = _minValue + (relativeAngle / arcLength) * (_maxValue - _minValue);
        }
        
        private void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button != 0)
                return;

            _state &= ~State.Active;
            MarkDirtyRepaint();
            
            this.ReleasePointer(0);

            _canMove = false;
        }
        
        private Vector2 GetCircularPosition2D(float angleDegrees, float radius, Vector2 center)
        {
            var angleRadians = angleDegrees * Mathf.Deg2Rad;
            
            var x = center.x + radius * Mathf.Cos(angleRadians);
            var y = center.y + radius * Mathf.Sin(angleRadians);
            
            return new Vector2(x, y);
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

        [Flags]
        private enum State
        {
            Normal = 1,
            Hover = 2,
            Active = 4
        }
    }
}