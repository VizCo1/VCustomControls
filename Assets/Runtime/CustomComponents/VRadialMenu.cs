using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialMenu : VisualElement, INotifyValueChanged<int>, IVHasCustomEvent
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly CustomStyleProperty<Color> RadialBackgroundColor = new("--radial-background-color");
        private static readonly CustomStyleProperty<Color> RadialBorderColor = new("--radial-border-color");
        private static readonly CustomStyleProperty<Color> RadialSegmentColor = new("--radial-segment-color");
        private static readonly CustomStyleProperty<float> RadialBorderWidth = new("--radial-border-width");
        
        public static readonly string RadialMenuClass = "radial-menu";
        public static readonly string RadialMenuSubmittedClass = "radial-menu-submitted";
        public static readonly string RadialMenuPostSubmittedClass = "radial-menu-post-submitted";
        
        public static readonly string ImageSlotName = "ImageSlot";
        public static readonly string ImageSlotClass = "image-slot";
        public static readonly string ImageSlotPosClass = ImageSlotClass + "-pos-";
        
        [Header(nameof(VRadialMenu))]
        
        public VCustomEventType CustomEvent { get; }
        
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
        private int SlotImageWidth { get; set; } = 75;
        
        [UxmlAttribute]
        private int SlotImageHeight { get; set; } = 75;
        
        [UxmlAttribute, Range(0f, 1f)]
        private float SlotImagePosition { get; set; } = 0.5f;

        private Color _radialBackgroundColor;
        private Color _radialBorderColor;
        private Color _radialSegmentColor;
        private float _radialBorderWidth;
        
        private int _value = -1;

        public event Action<int> OnSlotClicked;
        
        public VRadialMenu() 
        {
            generateVisualContent += OnGenerateVisualContent;

            AddToClassList(RadialMenuClass);
            CustomEvent |= VCustomEventType.AimEvent;
            CustomEvent |= VCustomEventType.PostSubmitEvent;

            var clickable = new Clickable(OnClicked);
            this.AddManipulator(clickable);
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChangedEvent);
            
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<NavigationSubmitEvent>(OnSubmitted);
            RegisterCallback<VAimEvent>(OnAimed);
            RegisterCallback<PostSubmitEvent>(OnPostSubmitted);
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            var angleSlot = 360f / Slots;
            var radius = (contentRect.width * 0.5f - _radialBorderWidth * 0.5f) * SlotImagePosition;
            
            for (var i = 0; i < Slots; i++)
            {
                CreateImageSlotElement(i, angleSlot, radius);
            }
        }

        public void SetValueWithoutNotify(int newValue)
        {
            _value = newValue.Clamp(-1, Slots - 1);
            
            MarkDirtyRepaint();
        }
        
        public override bool ContainsPoint(Vector2 localPoint)
        {
            var center = contentRect.center;
            var distance = Vector2.Distance(localPoint, center);
            var radius = contentRect.width * 0.5f;
            
            return distance < radius;
        }

        private void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            evt.customStyle.TryGetValue(RadialBackgroundColor, out _radialBackgroundColor);
            evt.customStyle.TryGetValue(RadialBorderColor, out _radialBorderColor);
            evt.customStyle.TryGetValue(RadialSegmentColor, out _radialSegmentColor);
            evt.customStyle.TryGetValue(RadialBorderWidth, out _radialBorderWidth);

            if (_radialBackgroundColor == null ||  _radialBorderColor == null || _radialSegmentColor == null)
                return;
            
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter2D = mgc.painter2D;
            
            painter2D.fillColor = _radialBackgroundColor;
            painter2D.strokeColor = _radialBorderColor;
            painter2D.lineWidth = _radialBorderWidth;
            
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

            if (_value == -1)
                return;
            
            var angleSlot = 360f / Slots;
            var previousAngle = angleSlot * _value;
            var nextAngle = previousAngle + angleSlot;
            
            painter2D.fillColor = _radialSegmentColor;
        
            var point1 = VMathExtensions.GetCircumferencePoint(previousAngle, radius, contentRect.center);
            var point2 = VMathExtensions.GetCircumferencePoint(nextAngle, radius, contentRect.center);
        
            DrawCircleSegment(painter2D, radius, center, point1, point2, previousAngle, nextAngle);
        }
        
        private void OnPointerMove(PointerMoveEvent evt)
        {
            var dir = (Vector2)evt.localPosition - contentRect.center;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            if (angle < 0)
            {
                angle += 360f;
            }

            SelectMatchingSegment(angle);
        }
        
        private void OnClicked()
        {
            OnSlotClicked?.Invoke(_value);
        }
        
        private void OnSubmitted(NavigationSubmitEvent evt)
        {
            AddToClassList(RadialMenuSubmittedClass);
        }
        
        private void OnPostSubmitted(PostSubmitEvent evt)
        {
            RemoveFromClassList(RadialMenuSubmittedClass);
            AddToClassList(RadialMenuPostSubmittedClass);
            
            schedule
                .Execute(() => RemoveFromClassList(RadialMenuPostSubmittedClass))
                .StartingIn(100)
                .Until(() => true);
            
            OnSlotClicked?.Invoke(_value);
        }

        private void OnAimed(VAimEvent evt)
        {
            if (evt.Aim == Vector2.zero)
                return;
            
            var angle = Mathf.Atan2(evt.Aim.y, evt.Aim.x) * Mathf.Rad2Deg;
            
            if (angle < 0)
            {
                angle += 360f;
            }
            
            SelectMatchingSegment(angle);
        }
        
        private void CreateImageSlotElement(int i, float angleSlot, float radius)
        {
            var imageElement = new VisualElement
            {
                name = ImageSlotName + i,
                style =
                {
                    position = Position.Absolute,
                    width = SlotImageWidth,
                    height = SlotImageHeight,
                }
            };
                
            var angle = angleSlot * i + angleSlot * 0.5f;
            var position = VMathExtensions.GetCircumferencePoint(angle, radius, contentRect.center);
            var offset = new Vector2(SlotImageWidth * 0.5f, SlotImageHeight * 0.5f);
                
            imageElement.style.translate = new Translate(position.x - offset.x, position.y - offset.y);
                
            imageElement.AddToClassList(ImageSlotClass);
            imageElement.AddToClassList(ImageSlotPosClass + i);
            Add(imageElement);
        }

        private void SelectMatchingSegment(float angle)
        {
            var angleSlot = 360f / Slots;
            value = (int)(angle / angleSlot);
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
    }
}