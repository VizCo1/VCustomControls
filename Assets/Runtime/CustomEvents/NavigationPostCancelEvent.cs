using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class NavigationPostCancelEvent : EventBase<NavigationPostCancelEvent>
    {
        public new static NavigationPostCancelEvent GetPooled()
        {
            var pooled = EventBase<NavigationPostCancelEvent>.GetPooled();
            return pooled;
        }
    }
}