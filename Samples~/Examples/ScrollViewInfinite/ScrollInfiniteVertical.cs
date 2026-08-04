using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;
using Random = UnityEngine.Random;

namespace Samples
{
    public class ScrollInfiniteVertical : VBaseView<ScrollViewInfiniteVerticalElements>
    {
        private const string InfiniteVerticalButton = "examples-button-container-infinite-vertical";
    
        private VisualTreeAsset _elementToAdd;
    
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.AddToScrollViewVertical.ExamplesButton.RegisterCallback<ClickEvent>(OnAddElementButtonClicked);
            Elements.RemoveScrollViewVertical.ExamplesButton.RegisterCallback<ClickEvent>(OnRemoveElementButtonClicked);
        }
        
        private void OnDestroy()
        {
            Elements.AddToScrollViewVertical.ExamplesButton.UnregisterCallback<ClickEvent>(OnAddElementButtonClicked);
            Elements.RemoveScrollViewVertical.ExamplesButton.UnregisterCallback<ClickEvent>(OnRemoveElementButtonClicked);
        }
        
        public void InitializeData(ScrollInfiniteViewData data)
        {
            _elementToAdd = data.ElementToAdd;
        }
        
        private void OnAddElementButtonClicked(ClickEvent evt)
        {
            var element = _elementToAdd.Instantiate();
        
            if (element.TryGetVisualElement<Button>(null, null, out var button))
            {
                button.text = "Dynamically added!!";
            }
        
            element.AddToClassList(InfiniteVerticalButton);
        
            Elements.VScrollViewInfiniteVertical.Add(element);
        }
        
        private void OnRemoveElementButtonClicked(ClickEvent evt)
        {
            var randomIndex = Random.Range(0, Elements.VScrollViewInfiniteVertical.childCount);
            Elements.VScrollViewInfiniteVertical.RemoveAt(randomIndex);
        }
    }
}
