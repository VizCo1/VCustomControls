using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class VNavigationPostCancelEvent : EventBase<VNavigationPostCancelEvent>
    {
        public new static VNavigationPostCancelEvent GetPooled()
        {
            var pooled = EventBase<VNavigationPostCancelEvent>.GetPooled();
            return pooled;
        }
    }
}