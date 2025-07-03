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

                if (_textElement != null)
                {
                    _textElement.text = _text;
                }
            }
        }

        [UxmlAttribute, Range(0, 10f)]
        private float ScrollSpeed
        {
            get => _scrollSpeed;
            set => _scrollSpeed = value;
        }

        [UxmlAttribute]
        private long ScrollRate { get; set; } = 10;
        
        [UxmlAttribute]
        private bool IsLoopable { get; set; }

        private IVisualElementScheduledItem _scheduledItem;
        private string _text = "Label";
        private float _scrollSpeed = 1;

        public VScrollableLabel() 
        {
            AddToClassList(ScrollableLabelContainerClass);
            
            _textElement = new TextElement();
            _textElement.AddToClassList(ScrollableLabelClass);
            _textElement.pickingMode = PickingMode.Ignore;
            
            Add(_textElement);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            _scrollSpeed *= -1f;
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            style.height = 
                _textElement.resolvedStyle.height + 
                resolvedStyle.paddingBottom + 
                resolvedStyle.paddingTop + 
                resolvedStyle.borderTopWidth + 
                resolvedStyle.borderBottomWidth;
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == PointerEnterEvent.TypeId())
            {
                OnPointerEnter();
            }
            else if (evt.eventTypeId == PointerLeaveEvent.TypeId())
            {
                OnPointerLeave();
            }
            else if (evt.eventTypeId == DetachFromPanelEvent.TypeId())
            {
                OnDetachedFromPanel();
            }
        }

        private void OnPointerEnter()
        {
            _scheduledItem?.Pause();
            
            if (!ShouldStartScrolling())
                return;
            
            _scheduledItem = schedule
                .Execute(() => ScrollText(_scrollSpeed))
                .Every(ScrollRate)
                .Until(ShouldStopScrolling);
        }

        private void OnPointerLeave()
        {
            _scheduledItem?.Pause();
            
            if (!ShouldStartScrolling())
                return;
            
            _scheduledItem = schedule
                .Execute(() => ScrollText(-_scrollSpeed))
                .Every(ScrollRate)
                .Until(ShouldStopInverseScrolling);
        }
        
        private void OnDetachedFromPanel()
        {
            _scheduledItem?.Pause();
        }

        private bool ShouldStopInverseScrolling()
        {
            if (!(_textElement.resolvedStyle.translate.x >= _scrollSpeed) || !(_textElement.resolvedStyle.translate.x <= -_scrollSpeed)) 
                return false;
            
            _textElement.style.translate = new Translate(0f, _textElement.resolvedStyle.translate.y);
                
            return true;
        }

        private bool ShouldStartScrolling()
        {
            if (_scrollSpeed != 0)
                return 
                    _textElement.resolvedStyle.width > 
                    resolvedStyle.width - resolvedStyle.paddingLeft - resolvedStyle.paddingRight - 
                    resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth;

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
            
            if ((int)_textElement.resolvedStyle.translate.x > (int)(resolvedStyle.width - _textElement.resolvedStyle.width - horizontalBorderAndPadding)) 
                return false;
            
            _textElement.style.translate = new Translate(
                resolvedStyle.width - _textElement.resolvedStyle.width - horizontalBorderAndPadding, 
                _textElement.resolvedStyle.translate.y);

            return true;

        }

        private void ScrollText(float scrollSpeed)
        {
            var nextTranslateX = new Length(_textElement.resolvedStyle.translate.x + scrollSpeed);
            
            if (nextTranslateX.value < -_textElement.resolvedStyle.width)
            {
                nextTranslateX = new Length((int)resolvedStyle.width);
            }
            
            _textElement.style.translate = new Translate(nextTranslateX, _textElement.resolvedStyle.translate.y);
        }
    }
}