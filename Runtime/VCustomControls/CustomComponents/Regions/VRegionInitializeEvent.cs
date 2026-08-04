using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class VRegionInitializeEvent : EventBase<VRegionInitializeEvent>
    {
        public GameObject ViewGameObject { get; private set; }

        public static VRegionInitializeEvent GetPooled(GameObject viewGameObject)
        {
            var pooled = EventBase<VRegionInitializeEvent>.GetPooled();
            
            pooled.ViewGameObject = viewGameObject;
            
            return pooled;
        }
    }
}