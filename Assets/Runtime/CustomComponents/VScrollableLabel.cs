using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollableLabel : VisualElement
    {
        public static readonly string ScrollableLabelClass = "scrollable-label";
        public static readonly string ScrollableLabelContainerClass = ScrollableLabelClass + "-container";

        private const long ScrollRate = 250;
        
        private readonly Label _label;

        [Header(nameof(VScrollableLabel))]
        [UxmlAttribute]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;

                if (_label != null)
                {
                    _label.text = _text;
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
        private bool IsLoopable { get; set; }

        private IVisualElementScheduledItem _scheduledItem;
        private string _text = "Label";
        private float _scrollSpeed = 1;

        public VScrollableLabel() 
        {
            AddToClassList(ScrollableLabelContainerClass);
            
            _label = new Label();
            _label.style.alignSelf = new StyleEnum<Align>(Align.FlexStart);
            _label.AddToClassList(ScrollableLabelClass);

            Add(_label);
            
            RegisterCallbackOnce<GeometryChangedEvent>(OnAttachedToPanel);
        }

        private void OnAttachedToPanel(GeometryChangedEvent evt)
        {
            style.height = _label.resolvedStyle.height + 
                           resolvedStyle.paddingBottom + 
                           resolvedStyle.paddingTop + 
                           resolvedStyle.borderTopWidth + 
                           resolvedStyle.borderBottomWidth;

            _scrollSpeed *= -1f;
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
            if (!(_label.resolvedStyle.translate.x >= _scrollSpeed) || !(_label.resolvedStyle.translate.x <= -_scrollSpeed)) 
                return false;
            
            _label.style.translate = new Translate(0f, _label.resolvedStyle.translate.y);
                
            return true;
        }

        private bool ShouldStartScrolling()
        {
            if (_scrollSpeed != 0)
                return 
                    _label.resolvedStyle.width > 
                    resolvedStyle.width - resolvedStyle.paddingLeft - resolvedStyle.paddingRight - 
                    resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth;

            return false;
        }
        
        private bool ShouldStopScrolling()
        {
            if (IsLoopable)
                return false;

            var shouldStopScrolling = true;
            if (_scrollSpeed < 0)
            {
                shouldStopScrolling = 
                    (int)_label.resolvedStyle.translate.x <= 
                    (int)(resolvedStyle.width - _label.resolvedStyle.width - 
                          resolvedStyle.paddingLeft - resolvedStyle.paddingRight - 
                          resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth);
            }

            if (shouldStopScrolling)
            {
                _label.style.translate = new Translate(
                    resolvedStyle.width - _label.resolvedStyle.width - resolvedStyle.paddingLeft - 
                    resolvedStyle.paddingRight - resolvedStyle.borderLeftWidth - resolvedStyle.borderRightWidth, 
                    _label.resolvedStyle.translate.y);
            }
            
            return shouldStopScrolling;
        }

        private void ScrollText(float scrollSpeed)
        {
            var nextTranslateX = new Length(_label.resolvedStyle.translate.x + scrollSpeed);
            switch (scrollSpeed)
            {
                case > 0 when nextTranslateX.value > resolvedStyle.width:
                    nextTranslateX = new Length((int)-_label.resolvedStyle.width);
                    break;
                case < 0 when nextTranslateX.value < -_label.resolvedStyle.width:
                    nextTranslateX = new Length((int)resolvedStyle.width);
                    break;
            }
            
            _label.style.translate = new Translate(nextTranslateX, _label.resolvedStyle.translate.y);
        }
    }
}