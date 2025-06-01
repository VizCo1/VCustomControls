using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollViewInfinite : ScrollView
    {
        private Direction _direction;
        private float _lowValue;
        private float _highValue;
        private float _scrollerOffset;
        private float _previousVerticalScrollerValue;
        
        public VScrollViewInfinite() 
        {
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
                
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
                    throw new ArgumentException("ScrollViewMode must be set to Vertical or Horizontal when using InfiniteMode");
            }
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
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
                    
                    offset += child.GetTotalHeight();
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
                    
                    offset += child.GetTotalWidth();
                }
            }
        }

        private void InfiniteModeScrollerOnValueChanged(float newValue)
        {
            _direction = newValue <= _previousVerticalScrollerValue ? Direction.Positive : Direction.Negative;
            
            _previousVerticalScrollerValue = newValue;

            CalculateScrollerOffset();
            
            if (newValue >= _highValue - _scrollerOffset && _direction == Direction.Negative)
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
            else if (newValue <= _lowValue + _scrollerOffset && _direction == Direction.Positive)
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
            if (_direction == Direction.Positive)
            {
                var element = contentContainer[0];
                if (mode == ScrollViewMode.Vertical)
                {
                    _scrollerOffset = element.GetTotalHeight() * 0.75f;
                }
                else
                {
                    _scrollerOffset = element.GetTotalWidth() * 0.75f;
                }
            }
            else
            {
                var element = contentContainer[childCount - 1];
                if (mode == ScrollViewMode.Vertical)
                {
                    _scrollerOffset = element.GetTotalHeight() * 0.75f;
                }
                else
                {
                    _scrollerOffset = element.GetTotalWidth() * 0.75f;
                }
            }
        }

        private void HandleHorizontalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildRealWidth = lastChild.GetTotalWidth();
                
            _lowValue -= firstChild.GetTotalWidth();
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

            var lastChildRealWidth = lastChild.GetTotalWidth();
                    
            _lowValue += firstChild.GetTotalWidth();
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

            var lastChildReal = lastChild.GetTotalHeight();
                
            _lowValue -= firstChild.GetTotalHeight();
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

            var lastChildRealHeight = lastChild.GetTotalHeight();
                    
            _lowValue += firstChild.GetTotalHeight();
            _highValue += lastChildRealHeight;
                
            verticalScroller.highValue = _highValue;
            verticalScroller.lowValue = _lowValue;
                    
            var offset = lastChild.resolvedStyle.translate.y + lastChildRealHeight;
            firstChild.style.translate = new Translate(new Length(0), new Length(offset));
                    
            firstChild.BringToFront();
        }

        private enum Direction
        {
            Positive,
            Negative,
        }
    }
}