using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator;
using VCustomComponents.Runtime;

public class ScrollInfiniteHorizontal : VBaseView<ScrollViewInfiniteHorizontalElements>
{
    private const string InfiniteHorizontalButton = "examples-button-container-infinite-horizontal";
    
    private VisualTreeAsset _elementToAdd;
    
    protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
    {
        Elements.AddToScrollViewHorizontal.ExamplesButton.RegisterCallback<ClickEvent>(OnAddElementButtonClicked);
        Elements.RemoveScrollViewHorizontal.ExamplesButton.RegisterCallback<ClickEvent>(OnRemoveElementButtonClicked);
    }
    
    private void OnDestroy()
    {
        Elements.AddToScrollViewHorizontal.ExamplesButton.UnregisterCallback<ClickEvent>(OnAddElementButtonClicked);
        Elements.RemoveScrollViewHorizontal.ExamplesButton.UnregisterCallback<ClickEvent>(OnRemoveElementButtonClicked);
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
    
        element.AddToClassList(InfiniteHorizontalButton);
    
        Elements.VScrollViewInfiniteHorizontal.Add(element);
    }
    
    private void OnRemoveElementButtonClicked(ClickEvent evt)
    {
        var randomIndex = Random.Range(0, Elements.VScrollViewInfiniteHorizontal.childCount);
        Elements.VScrollViewInfiniteHorizontal.RemoveAt(randomIndex);
    }
}
