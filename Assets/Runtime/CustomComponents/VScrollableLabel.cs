using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollableLabel : VisualElement
    {
        public static readonly string ScrollableLabelClass = "scrollable-label";
        public static readonly string ScrollableLabelContainerClass = ScrollableLabelClass + "-container";
        
        private readonly TextElement _textElement;

        [Header(nameof(VScrollableLabel))]
        [UxmlAttribute]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;

                if (_textElement == null) 
                    return;
                
                _textElement.text = _text;
            }
        }

        [UxmlAttribute, Range(0f, 10f)]
        private float ScrollSpeed { get; set; } = 1;

        [UxmlAttribute]
        private long ScrollRate { get; set; } = 10;
        
        [UxmlAttribute]
        private bool IsLoopable { get; set; }

        [UxmlAttribute, Tooltip("When true the user cannot interact with the label")]
        private bool IsAutomatic { get; set; }
        
        [UxmlAttribute]
        private long MsBetweenScrollsWhenAutomatic { get; set; } = 250;
        
        private IVisualElementScheduledItem _scheduledItem;
        private string _text = "Label";

        public VScrollableLabel() 
        {
            AddToClassList(ScrollableLabelContainerClass);
            
            _textElement = new TextElement();
            _textElement.AddToClassList(ScrollableLabelClass);
            _textElement.usageHints = UsageHints.DynamicTransform;
            _textElement.pickingMode = PickingMode.Ignore;
            
            Add(_textElement);
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            style.height = 
                _textElement.resolvedStyle.height + 
                resolvedStyle.paddingBottom + 
                resolvedStyle.paddingTop + 
                resolvedStyle.borderTopWidth + 
                resolvedStyle.borderBottomWidth;
#if UNITY_EDITOR
            if (panel.contextType == ContextType.Editor)
                return;
#endif
            if (IsAutomatic)
            {
                OnPointerEnter();
            }
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (!IsAutomatic && evt.eventTypeId == PointerEnterEvent.TypeId())
            {
                OnPointerEnter();
            }
            else if (!IsAutomatic && evt.eventTypeId == PointerLeaveEvent.TypeId())
            {
                OnPointerLeave();
            }
            else if (evt.eventTypeId == DetachFromPanelEvent.TypeId())
            {
                OnDetachedFromPanel();
            }
        }

        private void OnPointerEnter(long delay = 0)
        {
            _scheduledItem?.Pause();
            
            if (!ShouldStartScrolling())
                return;

            _scheduledItem = schedule
                .Execute(() => ScrollText(-ScrollSpeed))
                .Every(ScrollRate)
                .Until(ShouldStopScrolling)
                .StartingIn(delay);
        }

        private void OnPointerLeave(long delay = 0)
        {
            _scheduledItem?.Pause();
            
            if (!ShouldStartScrolling())
                return;

            _scheduledItem = schedule
                .Execute(() => ScrollText(ScrollSpeed))
                .Every(ScrollRate)
                .Until(ShouldStopInverseScrolling)
                .StartingIn(delay);
        }
        
        private void OnDetachedFromPanel()
        {
            _scheduledItem?.Pause();
        }

        private bool ShouldStopInverseScrolling()
        {
            if (!(_textElement.resolvedStyle.translate.x >= -ScrollSpeed) || !(_textElement.resolvedStyle.translate.x <= ScrollSpeed)) 
                return false;
            
            _textElement.style.translate = new Translate(0f, _textElement.resolvedStyle.translate.y);
                
            if (IsAutomatic)
            {
                OnPointerEnter(MsBetweenScrollsWhenAutomatic);
            }
            
            return true;
        }

        private bool ShouldStartScrolling()
        {
            if (ScrollSpeed != 0)
                return _textElement.resolvedStyle.width > this.GetTotalInnerWidth();
            
            return false;
        }
        
        private bool ShouldStopScrolling()
        {
            if (IsLoopable)
                return false;

            var horizontalBorderAndPadding = 
                resolvedStyle.paddingLeft + 
                resolvedStyle.paddingRight +
                resolvedStyle.borderLeftWidth + 
                resolvedStyle.borderRightWidth;

            var nextPos = (int)(resolvedStyle.width - _textElement.resolvedStyle.width - horizontalBorderAndPadding);
            
            if ((int)_textElement.resolvedStyle.translate.x > nextPos) 
                return false;
            
            _textElement.style.translate = new Translate(nextPos, _textElement.resolvedStyle.translate.y);

            if (IsAutomatic)
            {
                OnPointerLeave(MsBetweenScrollsWhenAutomatic);
            }
            
            return true;
        }

        private void ScrollText(float scrollSpeed)
        {
            var nextTranslateX = new Length(_textElement.resolvedStyle.translate.x + scrollSpeed);
            
            if (scrollSpeed < 0 && nextTranslateX.value < -_textElement.resolvedStyle.width)
            {
                nextTranslateX = new Length((int)resolvedStyle.width);
            }
            else if (scrollSpeed > 0 && nextTranslateX.value > resolvedStyle.width)
            {
                nextTranslateX = new Length((int)-_textElement.resolvedStyle.width);
            }
            
            _textElement.style.translate = new Translate(nextTranslateX, _textElement.resolvedStyle.translate.y);
        }
    }
}