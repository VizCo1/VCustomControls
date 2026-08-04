using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;

namespace Samples
{
    public class ScrollAnimatedVertical : VBaseView<ScrollViewAnimatedVerticalElements>
    {
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.EaseEnumDropdownContainerVertical.ExamplesEnumDropdown.value = Ease.Linear;
        
            Elements.ButtonContainerA.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, 0);
            Elements.ButtonContainerB.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, 1);
            Elements.ButtonContainerC.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, 2);
            Elements.ButtonContainerD.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, 3);
            Elements.ButtonContainerE.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked, 4);
            Elements.ButtonContainerF.ExamplesButton.RegisterCallback<ClickEvent, int>(OnVerticalButtonClicked,5);
        }
        
        private void OnVerticalButtonClicked(ClickEvent evt, int index)
        {
            var element = Elements.VScrollViewAnimatedVertical[index];
            Elements.VScrollViewAnimatedVertical.AnimatedScrollTo(
                element, 
                Elements.DurationFloatFieldContainerVertical.ExamplesFloatField.value, 
                (VAnimatedScrollType)Elements.AnimatedScrollTypeEnumDropdownContainerVertical.ExamplesEnumDropdown.value, 
                (Ease)Elements.EaseEnumDropdownContainerVertical.ExamplesEnumDropdown.value);
        }
    }
}