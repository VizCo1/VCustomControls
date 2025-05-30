using UnityEngine.UIElements;

namespace VCustomComponents
{
    public static class VisualElementExtensions
    {
        public static float GetRealHeight(this VisualElement element)
        {
            return element.resolvedStyle.height + element.resolvedStyle.marginTop + element.resolvedStyle.marginBottom;
        }
        
        public static float GetRealWidth(this VisualElement element)
        {
            return element.resolvedStyle.width + element.resolvedStyle.marginLeft + element.resolvedStyle.marginRight;
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

        public static bool TryGetVisualElement(
            this VisualElement element,
            string name,
            string className,
            out VisualElement visualElement)
        {
            return element.TryGetVisualElement<VisualElement>(name, className, out visualElement);
        }
        
        public static bool TryGetVisualElement<T>(
            this T element, 
            string name,
            string className,
            out T visualElement)  
            where T : VisualElement
        {
            visualElement = element.Q<T>();
            
            return visualElement != null;
        }
    }
}
