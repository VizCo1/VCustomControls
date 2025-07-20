using UnityEngine.UIElements;

namespace VCustomComponents
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