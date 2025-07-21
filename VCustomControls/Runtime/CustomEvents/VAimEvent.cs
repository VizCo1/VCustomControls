using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class VAimEvent : EventBase<VAimEvent>
    {
        public Vector2 Aim { get; private set; }
    
        public static VAimEvent GetPooled(Vector2 aimVector)
        {
            var pooled = EventBase<VAimEvent>.GetPooled();
            pooled.Aim = aimVector;
            return pooled;
        }

        public new static VAimEvent GetPooled()
        {
            var pooled = EventBase<VAimEvent>.GetPooled();
            pooled.Aim = Vector2.zero;
            return pooled;
        }
        
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }
        
        public VAimEvent()
        {
            LocalInit();
        }

        private void LocalInit()
        {
            Aim = Vector2.zero;
        }
    }
}
