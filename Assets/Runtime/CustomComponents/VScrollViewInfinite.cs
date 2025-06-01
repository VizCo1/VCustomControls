using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollViewInfinite : ScrollView
    {
        private const bool DoOneTime = true;
        
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
            touchScrollBehavior = TouchScrollBehavior.Unrestricted;
            
            horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            verticalScrollerVisibility = ScrollerVisibility.Hidden;
            
            switch (mode)
            {
                case ScrollViewMode.Vertical:
                    verticalScroller.valueChanged += OnScrollerValueChanged;
                    break;
                case ScrollViewMode.Horizontal:
                    horizontalScroller.valueChanged += OnScrollerValueChanged;
                    break;
                default:
                    throw new ArgumentException("ScrollViewMode must be set to Vertical or Horizontal");
            }
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (mode == ScrollViewMode.Vertical)
            {
                _highValue = verticalScroller.highValue;
                _lowValue = verticalScroller.lowValue;
                _previousVerticalScrollerValue =  verticalScroller.value;

                var offset = 0f;
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(0, offset);
                    
                    offset += child.GetTotalHeight();
                }
            }
            else
            {
                _highValue = horizontalScroller.highValue;
                _lowValue = horizontalScroller.lowValue;
                _previousVerticalScrollerValue =  horizontalScroller.value;
                
                var offset = 0f;
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(offset, 0);
                    
                    offset += child.GetTotalWidth();
                }
            }
        }

        private void OnScrollerValueChanged(float newValue)
        {
            if (childCount == 0)
                return;
            
            _direction = newValue <= _previousVerticalScrollerValue ? Direction.Positive : Direction.Negative;
            
            _previousVerticalScrollerValue = newValue;

            CalculateScrollerOffset();
            
            if (newValue + _scrollerOffset >= _highValue && _direction == Direction.Negative)
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
            else if (newValue - _scrollerOffset <= _lowValue && _direction == Direction.Positive)
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
            var index = 0;
            if (_direction == Direction.Negative)
            {
                index = childCount - 1;
            }
            
            var element = contentContainer[index];
            if (mode == ScrollViewMode.Vertical)
            {
                _scrollerOffset = element.GetTotalHeight();
            }
            else
            {
                _scrollerOffset = element.GetTotalWidth();
            }
        }

        private void HandleHorizontalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildTotalWidth = lastChild.GetTotalWidth();
                
            _lowValue -= lastChildTotalWidth;
            _highValue -= lastChildTotalWidth;
                
            var offset = firstChild.resolvedStyle.translate.x - lastChildTotalWidth;
            lastChild.style.translate = new Translate(offset, 0);
                
            lastChild.SendToBack();
        }

        private void HandleHorizontalDown()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var firstChildTotalWidth = firstChild.GetTotalWidth();
                    
            _lowValue += firstChildTotalWidth;
            _highValue += firstChildTotalWidth;
                    
            var offset = lastChild.resolvedStyle.translate.x + lastChild.GetTotalWidth();
            firstChild.style.translate = new Translate(offset, 0);
                    
            firstChild.BringToFront();
        }

        private void HandleVerticalUp()
        {
            Debug.Log("HandleVerticalUp");
            
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildTotalHeight = lastChild.GetTotalHeight();
                
            _lowValue -= lastChildTotalHeight;
            _highValue -= lastChildTotalHeight;
                
            var offset = firstChild.resolvedStyle.translate.y - lastChildTotalHeight;
            lastChild.style.translate = new Translate(0, offset);
            
            lastChild.SendToBack();
        }

        private void HandleVerticalDown()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var firstChildTotalHeight = firstChild.GetTotalHeight();
                    
            _lowValue += firstChildTotalHeight;
            _highValue += firstChildTotalHeight;
                    
            var offset = lastChild.resolvedStyle.translate.y + lastChild.GetTotalHeight();
            firstChild.style.translate = new Translate(0, offset);
                    
            firstChild.BringToFront();
        }

        public new void Add(VisualElement element)
        {
            VisualElement lastChild = null;
            if (childCount != 0)
            {
                lastChild = contentContainer.ElementAt(childCount - 1);
            }
            
            base.Add(element);
            
            element.style.position = Position.Absolute;
            element.SetVisibility(false);
            
            if (mode == ScrollViewMode.Vertical)
            {
                schedule
                    .Execute(() =>
                    {
                        var elementTotalHeight = element.GetTotalHeight();
                
                        _lowValue += elementTotalHeight;
                        _highValue += elementTotalHeight;

                        var offset = 0f;
                        if (lastChild != null)
                        {
                            offset = lastChild.resolvedStyle.translate.y + lastChild.GetTotalHeight();
                        }
                        
                        element.style.translate = new Translate(0, offset);
                        
                        element.SetVisibility(true);
                    })
                    .Until(() => DoOneTime);
            }
            else
            {
                schedule
                    .Execute(() =>
                    {
                        var elementTotalWidth = element.GetTotalWidth();
                
                        _lowValue += elementTotalWidth;
                        _highValue += elementTotalWidth;


                        var offset = 0f;
                        if (lastChild != null)
                        {
                            offset = lastChild.resolvedStyle.translate.x + lastChild.GetTotalWidth();
                        }
                        
                        element.style.translate =  new Translate(offset, 0);
                        
                        element.SetVisibility(true);
                    })
                    .Until(() => DoOneTime);
            }
        }

        public new void Remove(VisualElement elementToRemove)
        {
            if (!contentContainer.Contains(elementToRemove))
                throw new ArgumentException("The element to remove is not contained by the ScrollView.");
            
            RemoveAt(contentContainer.IndexOf(elementToRemove));
        }
        
        public new void RemoveAt(int index)
        {
            if (index >= childCount || index < 0)
                throw new IndexOutOfRangeException($"The index must be between 0 and {childCount - 1}.");
            
            var elementToRemove = contentContainer.ElementAt(index);

            if (mode == ScrollViewMode.Vertical)
            {
                var elementToRemoveTotalHeight = elementToRemove.GetTotalHeight();
                
                _highValue -= elementToRemoveTotalHeight;
                
                for (var i = index; i < childCount; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.y;
                    
                    child.style.translate = new Translate(0, childPosition - elementToRemoveTotalHeight);
                }
                
                base.RemoveAt(index);
                
                OnScrollerValueChanged(verticalScroller.value);
            }
            else
            {
                var elementToRemoveTotalWidth = elementToRemove.GetTotalWidth();
                
                _highValue -= elementToRemoveTotalWidth;
                
                for (var i = index; i < childCount; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.x;
                    
                    child.style.translate = new Translate(childPosition - elementToRemoveTotalWidth, 0);
                }
                
                base.RemoveAt(index);
                
                OnScrollerValueChanged(horizontalScroller.value);
            }
        }

        private enum Direction
        {
            Positive,
            Negative,
        }
    }
}