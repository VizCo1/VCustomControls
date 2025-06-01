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
        private const string RemoveElementButtonVertical = "RemoveScrollViewVertical";
        private const string RemoveElementButtonHorizontal = "RemoveScrollViewHorizontal";
        
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
            
            var addElementButtonVertical = verticalTab.Q<Button>();
            var addElementButtonHorizontal = horizontalTab.Q<Button>();
            
            addElementButtonVertical.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnAddElementButtonClicked, scrollViewInfiniteVertical);
            
            addElementButtonHorizontal.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnAddElementButtonClicked, scrollViewInfiniteHorizontal);
            
            var removeElementButtonVertical = (Button)verticalTab.Q(RemoveElementButtonVertical)[0];
            var removeElementButtonHorizontal = (Button)horizontalTab.Q(RemoveElementButtonHorizontal)[0];
            
            removeElementButtonVertical.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnRemoveElementButtonClicked, scrollViewInfiniteVertical);
            
            removeElementButtonHorizontal.RegisterCallback<ClickEvent, VScrollViewInfinite>
                (OnRemoveElementButtonClicked, scrollViewInfiniteHorizontal);
        }

        private void OnAddElementButtonClicked(ClickEvent evt, VScrollViewInfinite scrollViewInfinite)
        {
            var element = _elementToAdd.Instantiate();

            if (element.TryGetVisualElement<Button>(null, null, out var button))
            {
                button.text = "Dynamically added!!";
            }

            element.AddToClassList(scrollViewInfinite.mode == ScrollViewMode.Vertical
                ? InfiniteVerticalButton
                : InfiniteHorizontalButton);

            scrollViewInfinite.Add(element);
        }

        private void OnRemoveElementButtonClicked(ClickEvent evt, VScrollViewInfinite scrollViewInfinite)
        {
            // var randomIndex = Random.Range(0, scrollViewInfinite.childCount);
            var randomIndex = 0;
            scrollViewInfinite.RemoveAt(randomIndex);
        }
    }
}
