using System;

namespace VCustomComponents.Runtime
{
    [Flags]
    public enum VCustomEventType
    {
        None = 0,
        AimEvent = 1,
        PostSubmitEvent = 2,
        PostCancelEvent = 4
    }
}
