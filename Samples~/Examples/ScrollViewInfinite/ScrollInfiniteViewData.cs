using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class ScrollInfiniteViewData
    {
        public VisualTreeAsset ElementToAdd {private set; get; }

        public ScrollInfiniteViewData(VisualTreeAsset elementToAdd)
        {
            ElementToAdd = elementToAdd;
        }
    }
}