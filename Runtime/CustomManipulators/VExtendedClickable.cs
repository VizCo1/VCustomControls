using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    // The only purpose of this class is to have the :active pseudostate while being able to use the PointerDownEvent
    public class VExtendedClickable : Clickable
    {
        public event Action<PointerDownEvent> PointerDown;
        
        public VExtendedClickable(Action handler, long delay, long interval) : base(handler, delay, interval)
        {
        }

        public VExtendedClickable(Action<EventBase> handler) : base(handler)
        {
        }

        public VExtendedClickable(Action handler) : base(handler)
        {
        }

        public VExtendedClickable() : base((Action)null)
        {
            
        }
        
        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            base.ProcessDownEvent(evt, localPosition, pointerId);
            
            PointerDown?.Invoke((PointerDownEvent)evt);
        }
    }
}
