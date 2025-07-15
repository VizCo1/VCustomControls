using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VRadialMenu : VisualElement, INotifyValueChanged<int>, IVHasCustomEvent
    {
        public static readonly string RadialMenuClass = "radial-menu";
        public static readonly string RadialMenuSubmittedClass = "radial-menu-submitted";
        
        public static readonly string ImageSlotName = "ImageSlot";
        public static readonly string ImageSlotClass = "image-slot";
        public static readonly string ImageSlotPosClass = ImageSlotClass + "-pos-";
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        private static readonly CustomStyleProperty<Color> RadialBackgroundColor = new("--radial-background-color");
        private static readonly CustomStyleProperty<Color> RadialBorderColor = new("--radial-border-color");
        private static readonly CustomStyleProperty<Color> RadialSegmentColor = new("--radial-segment-color");
        private static readonly CustomStyleProperty<float> RadialBorderWidth = new("--radial-border-width");
        
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
        public int Slots
        {
            get => _slots;
            set
            {
                _slots = value;
                OnGeometryChangedEvent(null);
            }
        }

        [UxmlAttribute, Range(0f, 1f)]
        public float SlotImagePosition
        {
            get => _slotImagePosition;
            set
            {
                _slotImagePosition = value;
                OnGeometryChangedEvent(null);
            }
        }

        [UxmlAttribute]
        public float AngleOffset
        {
            get => _angleOffset;
            set
            {
                _angleOffset = value;
                OnGeometryChangedEvent(null);
            }
        }

        private Color _radialBackgroundColor;
        private Color _radialBorderColor;
        private Color _radialSegmentColor;
        private float _radialBorderWidth;
        
        private int _value = -1;
        private float _slotImagePosition = 0.5f;
        private int _slots = 2;
        private float _angleOffset;
        private bool _isAboutToSelect;
        
        public event Action<int> OnSlotClicked;
        
        public VRadialMenu() 
        {
            generateVisualContent += OnGenerateVisualContent;

            AddToClassList(RadialMenuClass);
            
            CustomEvent |= 
                VCustomEventType.AimEvent | 
                VCustomEventType.PostSubmitEvent | 
                VCustomEventType.PostCancelEvent;

            var clickable = new Clickable(OnClicked);
            this.AddManipulator(clickable);
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            contentContainer.Clear();
            
            var angleSlot = 360f / Slots;
            var radius = contentRect.width * 0.5f * SlotImagePosition;

            if (Slots == 1)
            {
                CreateImageSlotElement(0, 0, 0);
                return;
            }
            
            for (var i = 0; i < Slots; i++)
            {
                CreateImageSlotElement(i, angleSlot, radius);
            }
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == PointerMoveEvent.TypeId())
            {
                OnPointerMove((PointerMoveEvent)evt);
            }
            else if (evt.eventTypeId == NavigationSubmitEvent.TypeId())
            {
                OnSubmitted();
            }
            else if (evt.eventTypeId == VAimEvent.TypeId())
            {
                OnAimed((VAimEvent)evt);
            }
            else if (evt.eventTypeId == NavigationPostSubmitEvent.TypeId())
            {
                OnPostSubmitted();
            }
            else if (evt.eventTypeId == CustomStyleResolvedEvent.TypeId())
            {
                OnStylesResolved((CustomStyleResolvedEvent)evt);
            }
            else if (evt.eventTypeId == NavigationPostCancelEvent.TypeId())
            {
                OnPostCancel();
            }
        }

        public void SetValueWithoutNotify(int newValue)
        {
            _value = Mathf.Clamp(newValue, -1, Slots - 1);
            
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
            if (evt.customStyle.TryGetValue(RadialBackgroundColor, out var radialBackgroundColor))
            {
                _radialBackgroundColor = radialBackgroundColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialBorderColor, out var radialBorderColor))
            {
                _radialBorderColor = radialBorderColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialSegmentColor, out var radialSegmentColor))
            {
                _radialSegmentColor = radialSegmentColor;
            }
            
            if (evt.customStyle.TryGetValue(RadialBorderWidth, out var radialBorderWidth))
            {
                _radialBorderWidth = radialBorderWidth;
            }
            
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
            
            // Draw background border
            painter2D.BeginPath();
            painter2D.Arc(center, radius, 0, 360);
            painter2D.Stroke();
            painter2D.ClosePath();
            
            radius = contentRect.width * 0.5f - painter2D.lineWidth;
            
            // Draw background
            painter2D.BeginPath();
            painter2D.Arc(center, radius, 0, 360);
            painter2D.Fill();
            painter2D.ClosePath();

            if (_value == -1)
                return;
            
            // Draw selected slot sector
            var angleSlot = 360f / Slots;
            var previousAngle = angleSlot * _value + _angleOffset;
            var nextAngle = previousAngle + angleSlot;
            
            painter2D.fillColor = _radialSegmentColor;
        
            var point1 = VMathExtensions.GetCircumferencePoint(previousAngle, radius, contentRect.center);
            var point2 = VMathExtensions.GetCircumferencePoint(nextAngle, radius, contentRect.center);
            
            painter2D.BeginPath();
            painter2D.MoveTo(center);
            painter2D.LineTo(point1);
            painter2D.Arc(contentRect.center, radius, previousAngle, nextAngle);
            painter2D.LineTo(point2);
            painter2D.Fill();
            painter2D.ClosePath();
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
            if (value == -1)
                return;
            
            OnSlotClicked?.Invoke(_value);
        }
        
        private void OnSubmitted()
        {
            if (value == -1)
                return;
            
            _isAboutToSelect = true;
            
            AddToClassList(RadialMenuSubmittedClass);
        }
        
        private void OnPostSubmitted()
        {
            if (!_isAboutToSelect)
                return;

            _isAboutToSelect = false;
            
            RemoveFromClassList(RadialMenuSubmittedClass);
            
            OnSlotClicked?.Invoke(_value);
        }
        
        private void OnPostCancel()
        {
            RemoveFromClassList(RadialMenuSubmittedClass);
            _isAboutToSelect = false;
            value = -1;
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
        
        private void CreateImageSlotElement(int index, float angleSlot, float radius)
        {
            var imageElement = new VisualElement
            {
                name = ImageSlotName + index
            };

            imageElement.RegisterCallbackOnce((GeometryChangedEvent _) => 
                OnGeometryChangedImageSlot(imageElement, index, angleSlot, radius));
            
            Add(imageElement);
            
            imageElement.AddToClassList(ImageSlotClass);
            imageElement.AddToClassList(ImageSlotPosClass + index);
            
        }

        private void OnGeometryChangedImageSlot(VisualElement imageElement, int index, float angleSlot, float radius)
        {
            var angle = angleSlot * index + angleSlot * 0.5f + _angleOffset;
            var position = VMathExtensions.GetCircumferencePoint(angle, radius, contentRect.center);
            var offset = new Vector2(imageElement.resolvedStyle.width * 0.5f, imageElement.resolvedStyle.height * 0.5f);
                
            imageElement.style.translate = new Translate(position.x - offset.x, position.y - offset.y);
        }

        private void SelectMatchingSegment(float angle)
        {
            angle = (angle - _angleOffset + 360f) % 360f;

            var angleSlot = 360f / Slots;
            value = (int)(angle / angleSlot);
        }
    }
}