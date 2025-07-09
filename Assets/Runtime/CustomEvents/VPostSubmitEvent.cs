using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class NavigationPostSubmitEvent : EventBase<NavigationPostSubmitEvent>
    {
        public new static NavigationPostSubmitEvent GetPooled()
        {
            var pooled = EventBase<NavigationPostSubmitEvent>.GetPooled();
            return pooled;
        }
    }
}
