using System;
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
            _easeDropdownVertical = _document.rootVisualElement.Q<EnumField>(EaseEnumDropdownContainerVerticalName);
            _durationFloatFieldVertical =  _document.rootVisualElement.Q<FloatField>(DurationFloatFieldContainerVertical);
            
            _easeDropdownHorizontal = _document.rootVisualElement.Q<EnumField>(EaseEnumDropdownContainerHorizontal);
            _durationFloatFieldHorizontal =  _document.rootVisualElement.Q<FloatField>(DurationFloatFieldContainerHorizontal);
        }

        private void OnDestroy() 
        {
        
        }
    }
}
