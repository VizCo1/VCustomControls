using UnityEngine.UIElements;

namespace VCustomComponents
{
    public static class VVisualElementExtensions
    {
        public static float GetTotalOuterHeight(this VisualElement element, bool ignoreTemplateContainer = false)
        {
            var realElement = element;
            
            if (ignoreTemplateContainer)
            {
                realElement = element.hierarchy[0];
            }
            
            return realElement.resolvedStyle.height + realElement.resolvedStyle.marginTop + realElement.resolvedStyle.marginBottom;
        }
        
        public static float GetTotalInnerHeight(this VisualElement element, bool ignoreTemplateContainer = false)
        {
            var realElement = element;
            
            if (ignoreTemplateContainer)
            {
                realElement = element.hierarchy[0];
            }
            
            return 
                realElement.resolvedStyle.height - 
                realElement.resolvedStyle.paddingTop - 
                realElement.resolvedStyle.paddingBottom -
                realElement.resolvedStyle.borderTopWidth -
                realElement.resolvedStyle.borderBottomWidth;
        }
        
        public static float GetTotalOuterWidth(this VisualElement element, bool ignoreTemplateContainer = false)
        {
            var realElement = element;
            
            if (ignoreTemplateContainer)
            {
                realElement = element.hierarchy[0];
            }
            
            return realElement.resolvedStyle.width + realElement.resolvedStyle.marginLeft + realElement.resolvedStyle.marginRight;
        }
        
        public static float GetTotalInnerWidth(this VisualElement element, bool ignoreTemplateContainer = false)
        {
            var realElement = element;
            
            if (ignoreTemplateContainer)
            {
                realElement = element.hierarchy[0];
            }
            
            return 
                realElement.resolvedStyle.width - 
                realElement.resolvedStyle.paddingLeft - 
                realElement.resolvedStyle.paddingRight -
                realElement.resolvedStyle.borderLeftWidth -
                realElement.resolvedStyle.borderRightWidth;
        }

        public static void SetVisibility(this VisualElement element, bool isVisible)
        {
            element.style.visibility = isVisible 
                ? Visibility.Visible 
                : Visibility.Hidden;
        }

        public static void SetDisplay(this VisualElement element, bool isVisible)
        {
            element.style.display = isVisible 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        public static void ToggleFocus(this VisualElement element)
        {
            var focusedElement = element.focusController.focusedElement;
            
            if (focusedElement == element)
            {
                element.Blur();
            }
            else
            {
                element.Focus();
            }
        }

        public static bool TryGetVisualElement(
            this VisualElement element,
            string name,
            string className,
            out VisualElement visualElement)
        {
            return element.TryGetVisualElement<VisualElement>(name, className, out visualElement);
        }
        
        public static bool TryGetVisualElement<T>(
            this VisualElement element, 
            string name,
            string className,
            out T visualElement)  
            where T : VisualElement
        {
            visualElement = element.Q<T>(name, className);
            
            return visualElement != null;
        }
    }
}
