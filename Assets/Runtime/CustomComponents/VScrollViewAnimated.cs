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
        
        [Header(nameof(VScrollViewAnimated))]
        
        private TweenerCore<float, float, FloatOptions> _animationTween1D; 
        private TweenerCore<Vector2, Vector2, VectorOptions> _animationTween2D;

        private bool _isAnimating;
        private float _previousMouseWheelScrollSize;
        
        public VScrollViewAnimated() 
        {
            touchScrollBehavior = TouchScrollBehavior.Unrestricted;
            
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

        private void OnVerticalScrollerValueChanged(float newValue)
        {
            if (_isAnimating)
                return;
            
            verticalScroller.value = newValue.Clamp(verticalScroller.lowValue, verticalScroller.highValue);
        }
        
        private void OnHorizontalScrollerValueChanged(float newValue)
        {
            if (_isAnimating)
                return;
            
            horizontalScroller.value = newValue.Clamp(horizontalScroller.lowValue, horizontalScroller.highValue);
        }

        public void AnimatedScrollToElement(VisualElement element, float duration, Ease ease = Ease.Linear)
        {
            _animationTween1D?.Kill();
            _animationTween2D?.Kill();
            
            _isAnimating = true;
            _previousMouseWheelScrollSize = mouseWheelScrollSize;
            mouseWheelScrollSize = 0;
            verticalScroller.pickingMode = PickingMode.Ignore;
            horizontalScroller.pickingMode = PickingMode.Ignore;
            
            if (mode == ScrollViewMode.Vertical)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).y
                    .Clamp(verticalScroller.lowValue, verticalScroller.highValue);
                
                _animationTween1D = DOTween.To(
                    () => verticalScroller.value,
                    newValue =>
                    {
                        verticalScroller.value = newValue;
                    }, 
                    targetPosition,
                    duration)
                    .SetEase(ease)
                    .OnComplete(OnAnimationCompleted);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).x
                    .Clamp(horizontalScroller.lowValue, horizontalScroller.highValue);
                
                _animationTween1D = DOTween.To(
                    () => horizontalScroller.value,
                    newValue =>
                    {
                        horizontalScroller.value = newValue;
                    }, 
                    targetPosition,
                    duration)
                    .SetEase(ease)
                    .OnComplete(OnAnimationCompleted);
            }
            else
            {
                var lowLimit = new Vector2(horizontalScroller.lowValue, horizontalScroller.highValue);
                var highLimit = new Vector2(verticalScroller.lowValue, verticalScroller.highValue);
                
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position)
                    .Clamp(lowLimit, highLimit);

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
                    .OnComplete(OnAnimationCompleted);
            }
        }

        private void OnAnimationCompleted()
        {
            _isAnimating = false;
            mouseWheelScrollSize = _previousMouseWheelScrollSize;
            verticalScroller.pickingMode = PickingMode.Position;
            horizontalScroller.pickingMode = PickingMode.Position;
        }
    }
}