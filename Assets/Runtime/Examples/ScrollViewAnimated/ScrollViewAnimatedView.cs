using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class ScrollViewAnimatedView : MonoBehaviour
    {
        private const string EaseEnumDropdownContainerVerticalName = "EaseEnumDropdownContainerVertical";
        private const string DurationFloatFieldContainerVertical = "DurationFloatFieldContainerVertical";
        private const string EaseEnumDropdownContainerHorizontal = "EaseEnumDropdownContainerHorizontal";
        private const string DurationFloatFieldContainerHorizontal = "DurationFloatFieldContainerHorizontal";

        private const string ScrollViewAnimatedVerticalTabName = "ScrollViewAnimatedVertical";
        private const string ScrollViewAnimatedHorizontalTabName = "ScrollViewAnimatedHorizontal";
        
        private UIDocument _document;
        
        private EnumField _easeDropdownVertical;
        private EnumField _easeDropdownHorizontal;
        private FloatField _durationFloatFieldVertical;
        private FloatField _durationFloatFieldHorizontal;

        private VScrollViewAnimated _scrollViewAnimatedVertical;
        private VScrollViewAnimated _scrollViewAnimatedHorizontal;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = _document.rootVisualElement;
            
            _easeDropdownVertical = (EnumField)root.Q(EaseEnumDropdownContainerVerticalName)[0];
            _easeDropdownVertical.value = Ease.InOutElastic;
            _durationFloatFieldVertical =  (FloatField)root.Q(DurationFloatFieldContainerVertical)[0];
            
            _easeDropdownHorizontal = (EnumField)root.Q(EaseEnumDropdownContainerHorizontal)[0];
            _easeDropdownHorizontal.value = Ease.InOutElastic;
            _durationFloatFieldHorizontal =  (FloatField)root.Q(DurationFloatFieldContainerHorizontal)[0];
            
            var verticalTab = root.Q(ScrollViewAnimatedVerticalTabName);
            var horizontalTab = root.Q(ScrollViewAnimatedHorizontalTabName);
            
            _scrollViewAnimatedVertical = verticalTab.Q<VScrollViewAnimated>();
            _scrollViewAnimatedHorizontal = horizontalTab.Q<VScrollViewAnimated>();

            var index = 0;
            verticalTab.Query<Button>().ForEach(button =>
            {
                button.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, index);
                index++;
            });

            index = 0;
            horizontalTab.Query<Button>().ForEach(button =>
            {
                button.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, index);
                index++;
            });
        }

        private void OnVerticalButtonClicked(ClickEvent evt, int index)
        {
            var element = _scrollViewAnimatedVertical[index];
            _scrollViewAnimatedVertical.AnimatedScrollToElement(element, _durationFloatFieldVertical.value, (Ease)_easeDropdownVertical.value);
        }
        
        private void OnHorizontalButtonClicked(ClickEvent evt, int index)
        {
            var element = _scrollViewAnimatedVertical[index];
            _scrollViewAnimatedHorizontal.AnimatedScrollToElement(element, _durationFloatFieldHorizontal.value, (Ease)_easeDropdownHorizontal.value);
        }

        private void OnDestroy() 
        {
        
        }
    }
}
