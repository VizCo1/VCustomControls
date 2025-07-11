using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollViewAnimated : ScrollView
    {
        public static readonly string ScrollViewAnimatedClass = "scroll-view-animated";
        public static readonly string TargetElementClass = "scroll-view-animated-target";
        
        [Header(nameof(VScrollViewAnimated))] 
        
        [UxmlAttribute]
        private bool StopAnimationWhenScrolling { get; set; }
        
        [UxmlAttribute]
        private float MinDistanceForMaxDuration { get; set; } = 200f;
        
        private TweenerCore<float, float, FloatOptions> _animationTween1D; 
        private TweenerCore<Vector2, Vector2, VectorOptions> _animationTween2D;

        private bool _isAnimating;
        private float _previousMouseWheelScrollSize;
        private VisualElement _previousTarget;
        
        public VScrollViewAnimated() 
        {
            AddToClassList(ScrollViewAnimatedClass);
            touchScrollBehavior = TouchScrollBehavior.Unrestricted;
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanelEvent);
        }

        private void OnAttachedToPanelEvent(AttachToPanelEvent evt)
        {
            if (mode == ScrollViewMode.Vertical)
            {
                verticalScroller.valueChanged += OnVerticalScrollerValueChanged;
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                horizontalScroller.valueChanged += OnHorizontalScrollerValueChanged;
            }
            else
            {
                verticalScroller.valueChanged += OnVerticalScrollerValueChanged;
                horizontalScroller.valueChanged += OnHorizontalScrollerValueChanged;
            }
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (StopAnimationWhenScrolling && evt.eventTypeId == WheelEvent.TypeId())
            {
                OnMouseWheel();
            }
        }

        private void OnMouseWheel()
        {
            _animationTween1D?.Kill();
            _animationTween2D?.Kill();
        }

        private void OnVerticalScrollerValueChanged(float newValue)
        {
            if (_isAnimating)
                return;
            
            verticalScroller.value = Mathf.Clamp(newValue, verticalScroller.lowValue, verticalScroller.highValue);
        }
        
        private void OnHorizontalScrollerValueChanged(float newValue)
        {
            if (_isAnimating)
                return;
            
            horizontalScroller.value = Mathf.Clamp(newValue, horizontalScroller.lowValue, horizontalScroller.highValue);
        }

        public void AnimatedScrollTo(
            VisualElement element, 
            float maxDuration, 
            VAnimatedScrollType animatedScrollType = VAnimatedScrollType.Default, 
            Ease ease = Ease.Linear)
        {
            if (!contentContainer.Contains(element))
                throw new ArgumentException("Cannot scroll to a VisualElement that's not a child of the ScrollView contentContainer.");
            
            _animationTween1D?.Kill();
            _animationTween2D?.Kill();
            
            _isAnimating = true;
            verticalScroller.pickingMode = PickingMode.Ignore;
            horizontalScroller.pickingMode = PickingMode.Ignore;
            _previousMouseWheelScrollSize = mouseWheelScrollSize;
            
            mouseWheelScrollSize = 0;

            _previousTarget = element;
            
            var targetPosition = element
                .ChangeCoordinatesTo(contentContainer, element.contentRect.position);
            
            if (animatedScrollType == VAnimatedScrollType.Centered)
            {
                var elementCenter = 
                    targetPosition + 
                    new Vector2(element.resolvedStyle.width * 0.5f, element.resolvedStyle.height * 0.5f);
                    
                targetPosition = 
                    elementCenter - 
                    new Vector2(contentViewport.resolvedStyle.width * 0.5f, contentViewport.resolvedStyle.height * 0.5f);
            }
            
            targetPosition = new Vector2(
                Mathf.Clamp(targetPosition.x, horizontalScroller.lowValue, horizontalScroller.highValue), 
                Mathf.Clamp(targetPosition.y, verticalScroller.lowValue,  verticalScroller.highValue));
            
            var distance = new Vector2(
                Mathf.Abs(horizontalScroller.value - targetPosition.x), 
                Mathf.Abs(verticalScroller.value - targetPosition.y));
            
            if (mode == ScrollViewMode.Vertical)
            {
                if (distance.y < MinDistanceForMaxDuration)
                {
                    var t = distance.y / MinDistanceForMaxDuration;
                    maxDuration = Mathf.Lerp(0, maxDuration, t);
                }
                
                _animationTween1D = DOTween.To(
                    () => verticalScroller.value,
                    newValue =>
                    {
                        verticalScroller.value = newValue;
                    },
                    targetPosition.y,
                    maxDuration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        element.AddToClassList(TargetElementClass);
                    })
                    .OnKill(OnAnimationCompleted);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                if (distance.x < MinDistanceForMaxDuration)
                {
                    var t = distance.x / MinDistanceForMaxDuration;
                    maxDuration = Mathf.Lerp(0, maxDuration, t);
                }
                
                _animationTween1D = DOTween.To(
                    () => horizontalScroller.value,
                    newValue =>
                    {
                        horizontalScroller.value = newValue;
                    },
                    targetPosition.x,
                    maxDuration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        element.AddToClassList(TargetElementClass);
                    })
                    .OnKill(OnAnimationCompleted);
            }
            else
            {
                var greaterDistance = Mathf.Max(distance.x, distance.y);
                
                if (greaterDistance < MinDistanceForMaxDuration)
                {
                    var t = greaterDistance / MinDistanceForMaxDuration;
                    maxDuration = Mathf.Lerp(0, maxDuration, t);
                }
                
                _animationTween2D = DOTween.To(
                    () => new Vector2(horizontalScroller.value, verticalScroller.value),
                    newValue =>
                    {
                        horizontalScroller.value = newValue.x;
                        verticalScroller.value = newValue.y;
                    },
                    targetPosition,
                    maxDuration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        element.AddToClassList(TargetElementClass);
                    })
                    .OnKill(OnAnimationCompleted);
            }
        }
        
        private void OnAnimationCompleted()
        { 
            _isAnimating = false;
            
            mouseWheelScrollSize = _previousMouseWheelScrollSize;
            
            verticalScroller.pickingMode = PickingMode.Position;
            horizontalScroller.pickingMode = PickingMode.Position;
            
            _previousTarget.RemoveFromClassList(TargetElementClass);
        }
    }
}