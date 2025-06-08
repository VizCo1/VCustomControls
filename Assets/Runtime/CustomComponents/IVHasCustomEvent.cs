using JetBrains.Annotations;

namespace VCustomComponents
{
    public interface IVHasCustomEvent
    {
        [UsedImplicitly] 
        public CustomEventType CustomEvent { get; }
        
        public enum CustomEventType
        {
            AimEvent,
        }
    }
}
