using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollViewCustom : ScrollView
    {
        
        [Header(nameof(VScrollViewCustom))]
        
        [UxmlAttribute]
        private Mode ChosenMode { get; set; }
        
        private float _scrollerOffset;
        private float _previousVerticalScrollerValue;
        private float _lowValue;
        private float _highValue;
        private Direction _direction;
        
        public VScrollViewCustom() 
        {
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private int _index = 0;
        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Q)
            {
                ScrollToElement(contentContainer[_index], 2f, Ease.Linear);
            }
            else if (evt.keyCode == KeyCode.W)
            {
                _index++;
                
                if (_index >= contentContainer.childCount)
                    _index = 0;
                
                Debug.Log(_index);
            }

        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (ChosenMode == Mode.InfiniteMode)
            {
                RegisterCallbackOnce<GeometryChangedEvent>(InfiniteModeOnGeometryChanged);
                
                touchScrollBehavior = TouchScrollBehavior.Unrestricted;

                horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                verticalScrollerVisibility = ScrollerVisibility.Hidden;
                
                switch (mode)
                {
                    case ScrollViewMode.Vertical:
                        verticalScroller.valueChanged += InfiniteModeScrollerOnValueChanged;
                        break;
                    case ScrollViewMode.Horizontal:
                        horizontalScroller.valueChanged += InfiniteModeScrollerOnValueChanged;
                        break;
                    default:
                        Debug.LogError("ScrollViewMode must be set to Vertical or Horizontal");
                        break;
                }

                return;
            }

            if (ChosenMode == Mode.AnimatedMode)
            {
                
            }
        }
        
#region Infinite mode
        
        private void InfiniteModeOnGeometryChanged(GeometryChangedEvent evt)
        {
            var offset = 0f;

            if (mode == ScrollViewMode.Vertical)
            {
                _highValue = verticalScroller.highValue;
                _lowValue = verticalScroller.lowValue;
                _previousVerticalScrollerValue =  verticalScroller.value;
                
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(new Length(0), new Length(offset));
                    
                    offset += child.GetRealHeight();
                }
            }
            else
            {
                _highValue = horizontalScroller.highValue;
                _lowValue = horizontalScroller.lowValue;
                _previousVerticalScrollerValue =  horizontalScroller.value;
                
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(new Length(offset), new Length(0));
                    
                    offset += child.GetRealWidth();
                }
            }
        }

        private void InfiniteModeScrollerOnValueChanged(float newValue)
        {
            _direction = newValue <= _previousVerticalScrollerValue ? Direction.Up : Direction.Down;
            
            _previousVerticalScrollerValue = newValue;

            CalculateScrollerOffset();
            
            if (newValue >= _highValue - _scrollerOffset && _direction == Direction.Down)
            {
                if (mode == ScrollViewMode.Vertical)
                {
                    HandleVerticalDown();
                }
                else
                {
                    HandleHorizontalDown();
                }
            }
            else if (newValue <= _lowValue + _scrollerOffset && _direction == Direction.Up)
            {
                if (mode == ScrollViewMode.Vertical)
                {
                    HandleVerticalUp();
                }
                else
                {
                    HandleHorizontalUp();
                }
            }
        }

        private void CalculateScrollerOffset()
        {
            if (_direction == Direction.Up)
            {
                var element = contentContainer[0];
                if (mode == ScrollViewMode.Vertical)
                {
                    _scrollerOffset = element.GetRealHeight() * 0.75f;
                }
                else
                {
                    _scrollerOffset = element.GetRealWidth() * 0.75f;
                }
            }
            else
            {
                var element = contentContainer[childCount - 1];
                if (mode == ScrollViewMode.Vertical)
                {
                    _scrollerOffset = element.GetRealHeight() * 0.75f;
                }
                else
                {
                    _scrollerOffset = element.GetRealWidth() * 0.75f;
                }
            }
        }

        private void HandleHorizontalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildRealWidth = lastChild.GetRealWidth();
                
            _lowValue -= firstChild.GetRealWidth();
            _highValue -= lastChildRealWidth;
                
            horizontalScroller.highValue = _highValue;
            horizontalScroller.lowValue = _lowValue;
                
            var offset = firstChild.resolvedStyle.translate.x - lastChildRealWidth;
            lastChild.style.translate = new Translate(new Length(offset), new Length(0));
                
            lastChild.SendToBack();
        }

        private void HandleHorizontalDown()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildRealWidth = lastChild.GetRealWidth();
                    
            _lowValue += firstChild.GetRealWidth();
            _highValue += lastChildRealWidth;
                
            horizontalScroller.highValue = _highValue;
            horizontalScroller.lowValue = _lowValue;
                    
            var offset = lastChild.resolvedStyle.translate.x + lastChildRealWidth;
            firstChild.style.translate = new Translate(new Length(offset), new Length(0));
                    
            firstChild.BringToFront();
        }

        private void HandleVerticalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildReal = lastChild.GetRealHeight();
                
            _lowValue -= firstChild.GetRealHeight();
            _highValue -= lastChildReal;
                
            verticalScroller.highValue = _highValue;
            verticalScroller.lowValue = _lowValue;
                
            var offset = firstChild.resolvedStyle.translate.y - lastChildReal;
            lastChild.style.translate = new Translate(new Length(0), new Length(offset));
                
            lastChild.SendToBack();
        }

        private void HandleVerticalDown()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildRealHeight = lastChild.GetRealHeight();
                    
            _lowValue += firstChild.GetRealHeight();
            _highValue += lastChildRealHeight;
                
            verticalScroller.highValue = _highValue;
            verticalScroller.lowValue = _lowValue;
                    
            var offset = lastChild.resolvedStyle.translate.y + lastChildRealHeight;
            firstChild.style.translate = new Translate(new Length(0), new Length(offset));
                    
            firstChild.BringToFront();
        }

        private enum Direction
        {
            Up,
            Down,
        }
        
#endregion

#region Animated mode

        public void ScrollToElement(VisualElement element, float duration, Ease ease = Ease.Linear)
        {
            var targetPosition = 0f;
            if (mode == ScrollViewMode.Vertical)
            {
                var value = element.ChangeCoordinatesTo(contentContainer, element.contentRect.position);
                Debug.Log(value);
            }
            else if (mode == ScrollViewMode.Horizontal)
            {
                
            }
            
            DOTween.To(
                () => verticalScroller.value,
                newValue => verticalScroller.value = newValue, 
                targetPosition,
                duration)
                .SetEase(ease);
        }

#endregion

        private enum Mode
        {
            InfiniteMode,
            AnimatedMode,
        }
    }
}