using DG.Tweening;
using UnityEngine.UIElements;
using UserInterfaceGenerator;

namespace VCustomComponents.Runtime
{
    public class ScrollAnimatedHorizontal : VBaseView<ScrollViewAnimatedHorizontalElements>
    {
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.EaseEnumDropdownContainerHorizontal.ExamplesEnumDropdown.value = Ease.Linear;

            Elements.ButtonContainerA.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, 0);
            Elements.ButtonContainerB.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, 1);
            Elements.ButtonContainerC.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, 2);
            Elements.ButtonContainerD.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, 3);
            Elements.ButtonContainerE.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked, 4);
            Elements.ButtonContainerF.ExamplesButton.RegisterCallback<ClickEvent, int>(OnHorizontalButtonClicked,5);
        }
        
        private void OnHorizontalButtonClicked(ClickEvent evt, int index)
        {
            var element = Elements.VScrollViewAnimatedHorizontal[index];
            Elements.VScrollViewAnimatedHorizontal.AnimatedScrollTo(
                element, 
                Elements.DurationFloatFieldContainerHorizontal.ExamplesFloatField.value, 
                (VAnimatedScrollType)Elements.AnimatedScrollTypeEnumDropdownContainerHorizontal.ExamplesEnumDropdown.value, 
                (Ease)Elements.EaseEnumDropdownContainerHorizontal.ExamplesEnumDropdown.value);
        }
    }
}