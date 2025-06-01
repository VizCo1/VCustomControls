using System;
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

                        var lastChildPosY = lastChild != null
                            ? lastChild.resolvedStyle.translate.y
                            : 0f;
                        
                        var offset = lastChildPosY + lastChild.GetTotalHeight();
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
                
                        var lastChildPosX = lastChild != null
                            ? lastChild.resolvedStyle.translate.x
                            : 0f;
                        
                        var offset = lastChildPosX + lastChild.GetTotalWidth();
                        element.style.translate =  new Translate(offset, 0);
                        
                        element.SetVisibility(true);
                    })
                    .Until(() => DoOneTime);
            }
        }

        public new void Remove(VisualElement elementToRemove)
        {
            RemoveAt(contentContainer.IndexOf(elementToRemove));
        }
        
        public new void RemoveAt(int index)
        {
            var elementToRemove = contentContainer.ElementAt(index);

            if (mode == ScrollViewMode.Vertical)
            {
                var elementToRemoveTotalHeight = elementToRemove.GetTotalHeight();
                
                _lowValue -= elementToRemoveTotalHeight;
                _highValue -= elementToRemoveTotalHeight;
                
                var offset = 0f;
                for (var i = index; i < childCount - 1; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.y;
                    
                    offset -= elementToRemoveTotalHeight;
                    
                    child.style.translate = new Translate(0, childPosition - offset);
                }
            }
            else
            {
                var elementToRemoveTotalWidth = elementToRemove.GetTotalWidth();
                
                _lowValue -= elementToRemoveTotalWidth;
                _highValue -= elementToRemoveTotalWidth;
                
                var offset = 0f;
                for (var i = index; i < childCount - 1; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.x;
                    
                    offset -= elementToRemoveTotalWidth;
                    
                    child.style.translate = new Translate(childPosition - offset, 0);
                }
            }
            
            base.RemoveAt(index);
        }

        private enum Direction
        {
            Positive,
            Negative,
        }
    }
}