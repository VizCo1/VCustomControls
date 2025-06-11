using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    // The only purpose of this class is to have the :active pseudostate while being able to use the PointerDownEvent
    public class ExtendedClickable : Clickable
    {
        public Action<PointerDownEvent> PointerDown;
        
        public ExtendedClickable(Action handler, long delay, long interval) : base(handler, delay, interval)
        {
        }

        public ExtendedClickable(Action<EventBase> handler) : base(handler)
        {
        }

        public ExtendedClickable(Action handler) : base(handler)
        {
        }
        
        protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            base.ProcessDownEvent(evt, localPosition, pointerId);
            
            PointerDown?.Invoke((PointerDownEvent)evt);
        }
    }
}
