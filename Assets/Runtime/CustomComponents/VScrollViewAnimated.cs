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

        public void AnimatedScrollTo(VisualElement element, float duration, Ease ease = Ease.Linear)
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
            
            if (mode == ScrollViewMode.Vertical)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).y;
                    
                targetPosition = 
                    Mathf.Clamp(targetPosition, verticalScroller.lowValue, verticalScroller.highValue);

                _animationTween1D = DOTween.To(
                    () => verticalScroller.value,
                    newValue =>
                    {
                        verticalScroller.value = newValue;
                    },
                    targetPosition,
                    duration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        if (Mathf.Approximately(targetPosition, verticalScroller.value))
                        {
                            _animationTween1D.Kill();
                        }
                        else
                        {
                            element.AddToClassList(TargetElementClass);
                        }
                    })
                    .OnComplete(OnAnimationCompleted)
                    .OnKill(OnAnimationCompleted);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).x;
                    
                targetPosition = 
                    Mathf.Clamp(targetPosition, horizontalScroller.lowValue, horizontalScroller.highValue);

                _animationTween1D = DOTween.To(
                    () => horizontalScroller.value,
                    newValue =>
                    {
                        horizontalScroller.value = newValue;
                    },
                    targetPosition,
                    duration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        if (Mathf.Approximately(targetPosition, horizontalScroller.value))
                        {
                            _animationTween1D.Kill();
                        }
                        else
                        {
                            element.AddToClassList(TargetElementClass);
                        }
                    })
                    .OnComplete(OnAnimationCompleted)
                    .OnKill(OnAnimationCompleted);
            }
            else
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position);

                _animationTween2D = DOTween.To(
                    () => new Vector2(horizontalScroller.value, verticalScroller.value),
                    newValue =>
                    {
                        horizontalScroller.value = newValue.x;
                        verticalScroller.value = newValue.y;
                    },
                    targetPosition,
                    duration)
                    .SetEase(ease)
                    .OnStart(() =>
                    {
                        if (Mathf.Approximately(targetPosition.x, horizontalScroller.value) &&
                            Mathf.Approximately(targetPosition.y, verticalScroller.value))
                        {
                            _animationTween2D.Kill();
                        }
                        else
                        {
                            element.AddToClassList(TargetElementClass);
                        }
                    })
                    .OnComplete(OnAnimationCompleted)
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