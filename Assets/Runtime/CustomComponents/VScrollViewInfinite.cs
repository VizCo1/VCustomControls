using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollViewInfinite : ScrollView
    {
        private const bool DoOneTime = true;
        
        private float _lowValue;
        private float _highValue;
        
        public VScrollViewInfinite() 
        {
            touchScrollBehavior = TouchScrollBehavior.Unrestricted;
            horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            verticalScrollerVisibility = ScrollerVisibility.Hidden;
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            switch (mode)
            {
                case ScrollViewMode.Vertical:
                    verticalScroller.valueChanged += OnScrollerValueChanged;
                    break;
                case ScrollViewMode.Horizontal:
                    horizontalScroller.valueChanged += OnScrollerValueChanged;
                    break;
                default:
                    Debug.LogError("ScrollViewMode must be set to Vertical or Horizontal");
                    break;
            }
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (mode == ScrollViewMode.Vertical)
            {
                _highValue = verticalScroller.highValue;
                _lowValue = verticalScroller.lowValue;

                var offset = 0f;
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(0, offset);
                    
                    offset += child.GetTotalOuterHeight();
                }
            }
            else
            {
                _highValue = horizontalScroller.highValue;
                _lowValue = horizontalScroller.lowValue;
                
                var offset = 0f;
                foreach (var child in contentContainer.Children())
                {
                    child.style.position = Position.Absolute;
                    child.style.translate =  new Translate(offset, 0);
                    
                    offset += child.GetTotalOuterWidth();
                }
            }
        }

        private void OnScrollerValueChanged(float newValue)
        {
            if (childCount == 0)
                return;
            
            while (newValue >= _highValue)
            {
                if (mode == ScrollViewMode.Vertical)
                    HandleVerticalDown();
                else
                    HandleHorizontalDown();
            }

            while (newValue <= _lowValue)
            {
                if (mode == ScrollViewMode.Vertical)
                    HandleVerticalUp();
                else
                    HandleHorizontalUp();
            }
        }

        private void HandleHorizontalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildTotalWidth = lastChild.GetTotalOuterWidth();
                
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

            var firstChildTotalWidth = firstChild.GetTotalOuterWidth();
                    
            _lowValue += firstChildTotalWidth;
            _highValue += firstChildTotalWidth;
                    
            var offset = lastChild.resolvedStyle.translate.x + lastChild.GetTotalOuterWidth();
            firstChild.style.translate = new Translate(offset, 0);
                    
            firstChild.BringToFront();
        }

        private void HandleVerticalUp()
        {
            var firstChild = contentContainer[0];
            var lastChild = contentContainer[childCount - 1];

            var lastChildTotalHeight = lastChild.GetTotalOuterHeight();
                
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

            var firstChildTotalOuterHeight = firstChild.GetTotalOuterHeight();
                    
            _lowValue += firstChildTotalOuterHeight;
            _highValue += firstChildTotalOuterHeight;
                    
            var offset = lastChild.resolvedStyle.translate.y + lastChild.GetTotalOuterHeight();
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
                        var elementTotalOuterHeight = element.GetTotalOuterHeight();

                        _highValue += elementTotalOuterHeight;

                        var offset = 0f;
                        if (lastChild != null)
                        {
                            offset = lastChild.resolvedStyle.translate.y + lastChild.GetTotalOuterHeight();
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
                        var elementTotalOuterWidth = element.GetTotalOuterWidth();

                        _highValue += elementTotalOuterWidth;

                        var offset = 0f;
                        if (lastChild != null)
                        {
                            offset = lastChild.resolvedStyle.translate.x + lastChild.GetTotalOuterWidth();
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
                throw new ArgumentException("The element to remove is not in the ScrollView contentContainer.");
            
            RemoveAt(contentContainer.IndexOf(elementToRemove));
        }
        
        public new void RemoveAt(int index)
        {
            if (index >= childCount || index < 0)
                throw new IndexOutOfRangeException($"The index must be between 0 and {childCount - 1}.");
            
            var elementToRemove = contentContainer.ElementAt(index);

            if (mode == ScrollViewMode.Vertical)
            {
                var elementToRemoveTotalOuterHeight = elementToRemove.GetTotalOuterHeight();

                _highValue -= elementToRemoveTotalOuterHeight;
                
                for (var i = index; i < childCount; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.y;
                    
                    child.style.translate = new Translate(0, childPosition - elementToRemoveTotalOuterHeight);
                }
            }
            else
            {
                var elementToRemoveTotalOuterWidth = elementToRemove.GetTotalOuterWidth();

                _highValue -= elementToRemoveTotalOuterWidth;
                
                for (var i = index; i < childCount; i++)
                {
                    var child = contentContainer.ElementAt(i);
                    var childPosition = child.resolvedStyle.translate.x;
                    
                    child.style.translate = new Translate(childPosition - elementToRemoveTotalOuterWidth, 0);
                }
            }
            
            base.RemoveAt(index);
        }
    }
}