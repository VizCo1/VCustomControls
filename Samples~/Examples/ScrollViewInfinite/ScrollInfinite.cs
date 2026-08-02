using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator;

namespace VCustomComponents.Runtime
{
    public class ScrollInfinite : VBaseView<ScrollViewInfiniteElements>
    {
        [SerializeField]
        private VisualTreeAsset _elementToAdd;
        
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            var viewData = new ScrollInfiniteViewData(_elementToAdd);
            
            Elements.VRegionHorizontal.RegisterCallbackOnce<VRegionInitializeEvent>(evt =>
            {
                var horizontal = evt.ViewGameObject.GetComponent<ScrollInfiniteHorizontal>();
                horizontal.InitializeData(viewData);
            });
            
            Elements.VRegionVertical.RegisterCallbackOnce<VRegionInitializeEvent>(evt =>
            {
                var vertical = evt.ViewGameObject.GetComponent<ScrollInfiniteVertical>();
                vertical.InitializeData(viewData);
            });
        }
    }
}
