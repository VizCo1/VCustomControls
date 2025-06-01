using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class ScrollViewInfiniteView : MonoBehaviour
    {
        private const string VerticalTabName = "ScrollViewInfiniteVertical";
        private const string HorizontalTabName = "ScrollViewInfiniteHorizontal";
        private const string InfiniteVerticalButton = "examples-button-container-infinite-vertical";
        private const string InfiniteHorizontalButton = "examples-button-container-infinite-horizontal";
        
        [SerializeField]
        private VisualTreeAsset _elementToAdd;
        
        private UIDocument _document;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = _document.rootVisualElement;
            
            var verticalTab = root.Q(VerticalTabName);
            var horizontalTab = root.Q(HorizontalTabName);

            var scrollViewInfiniteVertical = verticalTab.Q<VScrollViewInfinite>();
            var scrollViewInfiniteHorizontal = horizontalTab.Q<VScrollViewInfinite>();
            
            var buttonVertical = verticalTab.Q<Button>();
            var buttonHorizontal = horizontalTab.Q<Button>();
            
            buttonVertical.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnAddElementButtonClicked, scrollViewInfiniteVertical);
            
            buttonHorizontal.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnAddElementButtonClicked, scrollViewInfiniteHorizontal);
        }

        private void OnAddElementButtonClicked(ClickEvent evt, VScrollViewInfinite scrollViewInfinite)
        {
            var element = _elementToAdd.Instantiate();

            if (element.TryGetVisualElement<Button>(null, null, out var button))
            {
                button.text = "Dynamically added!!";
            }

            if (scrollViewInfinite.mode == ScrollViewMode.Vertical)
            {
                element.AddToClassList(InfiniteVerticalButton);
            }
            else
            {
                element.AddToClassList(InfiniteHorizontalButton);
            }
            
            scrollViewInfinite.Add(element);
        }
    }
}
