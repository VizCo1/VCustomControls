using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollableLabel : VisualElement
    {
        public static readonly string ScrollableLabelClass = "scrollable-label";
        public static readonly string ScrollableLabelContainerClass = ScrollableLabelClass + "-container";

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

        [UxmlAttribute]
        private Vector2 ScrollSpeed
        {
            get => _scrollSpeed;
            set
            {
                _scrollSpeed = value;
                _inverseScrollSpeed = -_scrollSpeed;
            }
        }
        
        [UxmlAttribute, Tooltip("This only affects the X axis, the Y axis is always loopable")]
        private bool IsLoopable { get; set; }

        private IVisualElementScheduledItem _scheduledItem;
        private Vector2 _scrollSpeed = Vector2.left;
        private Vector2 _inverseScrollSpeed;
        private string _text = "Label";

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
            style.height = _label.resolvedStyle.height + resolvedStyle.paddingBottom + resolvedStyle.paddingTop;
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

            _scheduledItem = schedule
                .Execute(() => ScrollText(ScrollSpeed))
                .Until(ShouldStopScrolling);
        }

        private void OnPointerLeave()
        {
            _scheduledItem.Pause();

            _scheduledItem = schedule
                .Execute(() => ScrollText(_inverseScrollSpeed))
                .Until(() => _label.resolvedStyle.translate == Vector3.zero);
        }

        private void OnDetachedFromPanel()
        {
            _scheduledItem?.Pause();
        }

        private bool ShouldStopScrolling()
        {
            if (IsLoopable)
                return false;

            return (int)_label.resolvedStyle.translate.x == (int)(resolvedStyle.width - _label.resolvedStyle.width - resolvedStyle.paddingLeft - resolvedStyle.paddingRight);
        }

        private void ScrollText(Vector2 scrollSpeed)
        {
            var nextTranslateX = new Length(_label.resolvedStyle.translate.x + scrollSpeed.x);
            switch (scrollSpeed.x)
            {
                case > 0 when nextTranslateX.value > resolvedStyle.width:
                    nextTranslateX = new Length((int)-_label.resolvedStyle.width);
                    break;
                case < 0 when nextTranslateX.value < -_label.resolvedStyle.width:
                    nextTranslateX = new Length((int)resolvedStyle.width);
                    break;
            }
            
            var nextTranslateY = new Length(_label.resolvedStyle.translate.y + scrollSpeed.y);
            switch (scrollSpeed.y)
            {
                case > 0 when nextTranslateY.value > resolvedStyle.height:
                    nextTranslateY = new Length((int)-_label.resolvedStyle.height);
                    break;
                case < 0 when nextTranslateY.value < -_label.resolvedStyle.height:
                    nextTranslateY = new Length((int)resolvedStyle.height);
                    break;
            }
            
            _label.style.translate = new Translate(nextTranslateX, nextTranslateY);
        }
    }
}