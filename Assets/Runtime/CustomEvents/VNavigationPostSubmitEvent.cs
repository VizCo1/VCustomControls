using UnityEngine.UIElements;

namespace VCustomComponents
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
