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
        
        private float _lowValue;
        private float _highValue;
        private float _scrollerOffset;
        private float _previousVerticalScrollerValue;
        
        private TweenerCore<float, float, FloatOptions> _animationTween1D; 
        private TweenerCore<Vector2, Vector2, VectorOptions> _animationTween2D; 
        
        public VScrollViewAnimated() 
        {
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private int _index = 0;
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Q)
            {
                Debug.Log(name + " Moving");
                ScrollToElement(contentContainer[_index], 2f, Ease.Linear);
            }
            else if (evt.keyCode == KeyCode.E)
            {
                _index++;
                
                if (_index >= contentContainer.childCount)
                    _index = 0;
                
                Debug.Log(name + " " + _index);
            }
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (touchScrollBehavior == TouchScrollBehavior.Unrestricted)
            {
                Debug.LogError("TouchScrollBehavior can't be Unrestricted when using AnimatedMode");
            }
        }
        
        public void ScrollToElement(VisualElement element, float duration, Ease ease = Ease.Linear)
        {
            _animationTween1D?.Kill();
            _animationTween2D?.Kill();
            
            if (mode == ScrollViewMode.Vertical)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).y
                    .Clamp(verticalScroller.lowValue, verticalScroller.highValue);
                
                _animationTween1D = DOTween.To(
                    () => verticalScroller.value,
                    newValue => verticalScroller.value = newValue, 
                    targetPosition,
                    duration)
                    .SetEase(ease);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                var targetPosition = element
                    .ChangeCoordinatesTo(contentContainer, element.contentRect.position).x
                    .Clamp(horizontalScroller.lowValue, horizontalScroller.highValue);
                
                _animationTween1D = DOTween.To(
                    () => horizontalScroller.value,
                    newValue => horizontalScroller.value = newValue, 
                    targetPosition,
                    duration)
                    .SetEase(ease);
            }
            else
            {
                var horizontalLimit = new Vector2(horizontalScroller.lowValue, horizontalScroller.highValue);
                var verticalLimit = new Vector2(verticalScroller.lowValue, verticalScroller.highValue);
                
                var targetPosition = element.ChangeCoordinatesTo(contentContainer, element.contentRect.position).Clamp(horizontalLimit, verticalLimit);

                _animationTween2D = DOTween.To(
                    () => new Vector2(horizontalScroller.value, verticalScroller.value),
                    newValue =>
                    {
                        horizontalScroller.value = newValue.x;
                        verticalScroller.value = newValue.y;
                    },
                    targetPosition,
                    duration)
                    .SetEase(ease);
            }
        }
    }
}