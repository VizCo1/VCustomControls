using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class PostSubmitEvent : EventBase<PostSubmitEvent>
    {
        public new static PostSubmitEvent GetPooled()
        {
            var pooled = EventBase<PostSubmitEvent>.GetPooled();
            return pooled;
        }
    }
}
