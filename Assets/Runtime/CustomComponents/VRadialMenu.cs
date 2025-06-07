using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialMenu : VisualElement, INotifyValueChanged<int>
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);

        [Header(nameof(VRadialMenu))]
        
        [UxmlAttribute, CreateProperty]
        public int value
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

                using var pooled = ChangeEvent<int>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }
        
        [UxmlAttribute]
        private int Slots { get; set; } = 2;

        [UxmlAttribute]
        private Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        private Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        private Color SegmentColor
        {
            get => _segmentColor;
            set
            {
                _segmentColor = value;
                MarkDirtyRepaint();
            }
        }
        
        [UxmlAttribute]
        private int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                MarkDirtyRepaint();
            }
        }

        private int _value;
        private Color _backgroundColor = Color.white;
        private Color _borderColor = Color.gray;
        private Color _segmentColor = Color.dimGray;
        private int _borderWidth = 10;
        
        public VRadialMenu() 
        {
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;
            
            painter2D.fillColor = BackgroundColor;
            painter2D.strokeColor = BorderColor;
            painter2D.lineWidth = BorderWidth;
            
            var center = contentRect.center;
            var radius = contentRect.width * 0.5f - painter2D.lineWidth * 0.5f;
            
            painter2D.BeginPath();
            painter2D.Arc(center, radius, 0, 360);
            painter2D.Stroke();
            painter2D.ClosePath();

            radius = contentRect.width * 0.5f - painter2D.lineWidth;
            
            painter2D.BeginPath();
            painter2D.Arc(center, radius, 0, 360);
            painter2D.Fill();
            painter2D.ClosePath();

            var angleSlot = 360f / Slots;
            var previousAngle = angleSlot * _value;
            var nextAngle = previousAngle + angleSlot;
            
            painter2D.fillColor = SegmentColor;
        
            var point1 = VMathExtensions.GetCircumferencePoint(previousAngle, radius, contentRect.center);
            var point2 = VMathExtensions.GetCircumferencePoint(nextAngle, radius, contentRect.center);
        
            DrawCircleSegment(painter2D, radius, center, point1, point2, previousAngle, nextAngle);
            
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
        
        }
    
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
        
        }

        private void DrawCircleSegment(
            Painter2D painter2D, 
            float radius, 
            Vector2 center, 
            Vector2 point1, 
            Vector2 point2, 
            float angle1, 
            float angle2)
        {
            painter2D.BeginPath();
            painter2D.MoveTo(center);
            painter2D.LineTo(point1);
            painter2D.Arc(contentRect.center, radius, angle1, angle2);
            painter2D.LineTo(point2);
            painter2D.Fill();
            painter2D.ClosePath();
        }
        
        public void SetValueWithoutNotify(int newValue)
        {
            _value = newValue.Clamp(0, Slots - 1);
            
            MarkDirtyRepaint();
        }
    }
}