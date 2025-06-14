using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class ScrollViewAnimatedView : BaseView
    {
        private const string EaseEnumDropdownContainerVerticalName = "EaseEnumDropdownContainerVertical";
        private const string DurationFloatFieldContainerVertical = "DurationFloatFieldContainerVertical";
        private const string EaseEnumDropdownContainerHorizontal = "EaseEnumDropdownContainerHorizontal";
        private const string DurationFloatFieldContainerHorizontal = "DurationFloatFieldContainerHorizontal";

        private const string ScrollViewAnimatedVerticalTabName = "ScrollViewAnimatedVertical";
        private const string ScrollViewAnimatedHorizontalTabName = "ScrollViewAnimatedHorizontal";
        private const string RowContainerName = "RowContainer";
        
        private EnumField _easeDropdownVertical;
        private EnumField _easeDropdownHorizontal;
        private FloatField _durationFloatFieldVertical;
        private FloatField _durationFloatFieldHorizontal;

        private VScrollViewAnimated _scrollViewAnimatedVertical;
        private VScrollViewAnimated _scrollViewAnimatedHorizontal;

        protected override void Start()
        {
            base.Start();
            
            _easeDropdownVertical = (EnumField)Root.Q(EaseEnumDropdownContainerVerticalName)[0];
            _easeDropdownVertical.value = Ease.Linear;
            _durationFloatFieldVertical =  (FloatField)Root.Q(DurationFloatFieldContainerVertical)[0];
            
            _easeDropdownHorizontal = (EnumField)Root.Q(EaseEnumDropdownContainerHorizontal)[0];
            _easeDropdownHorizontal.value = Ease.Linear;
            _durationFloatFieldHorizontal =  (FloatField)Root.Q(DurationFloatFieldContainerHorizontal)[0];
            
            var verticalTab = Root.Q(ScrollViewAnimatedVerticalTabName);
            var horizontalTab = Root.Q(ScrollViewAnimatedHorizontalTabName);
            
            _scrollViewAnimatedVertical = verticalTab.Q<VScrollViewAnimated>();
            _scrollViewAnimatedHorizontal = horizontalTab.Q<VScrollViewAnimated>();

            var index = 0;
            verticalTab.Q(RowContainerName).Query<Button>().ForEach(button =>
            {
                button.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, index);
                index++;
            });

            index = 0;
            horizontalTab.Q(RowContainerName).Query<Button>().ForEach(button =>
            {
                button.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, index);
                index++;
            });
        }

        private void OnVerticalButtonClicked(ClickEvent evt, int index)
        {
            var element = _scrollViewAnimatedVertical[index];
            _scrollViewAnimatedVertical.AnimatedScrollTo(element, _durationFloatFieldVertical.value, (Ease)_easeDropdownVertical.value);
        }
        
        private void OnHorizontalButtonClicked(ClickEvent evt, int index)
        {
            var element = _scrollViewAnimatedHorizontal[index];
            _scrollViewAnimatedHorizontal.AnimatedScrollTo(element, _durationFloatFieldHorizontal.value, (Ease)_easeDropdownHorizontal.value);
        }
    }
}
