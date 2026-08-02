using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class VNavigationPostSubmitEvent : EventBase<VNavigationPostSubmitEvent>
    {
        public new static VNavigationPostSubmitEvent GetPooled()
        {
            var pooled = EventBase<VNavigationPostSubmitEvent>.GetPooled();
            return pooled;
        }
    }
}
