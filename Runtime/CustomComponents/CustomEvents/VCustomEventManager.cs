using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public static class VCustomEventManager
    {
        private static readonly Dictionary<IPanel, VInputActionUI> InputActions = new();
        
        public static bool TryGetInputActionUI(this IPanel panel, out VInputActionUI inputAction)
        {
            inputAction = null;

            return InputActions.TryGetValue(panel, out inputAction);
        }

        public static bool TryRegisterInputActionUI(this IPanel panel, out VInputActionUI inputAction)
        {
            inputAction = null;
            
            inputAction = new VInputActionUI();
            inputAction.UI.Enable();

            if (!InputActions.TryAdd(panel, inputAction))
            {
                Debug.LogWarning($"The panel {panel.visualTree.name} already contains a VInputActionUI");
                return false;
            }
            
            return true;
        }
        
        public static bool TryUnregisterInputActionUI(this IPanel panel)
        {
            if (!InputActions.Remove(panel, out var inputAction))
            {
                Debug.LogWarning($"No VInputActionUI found for the panel {panel.visualTree.name}");
                return false;
            }
            
            inputAction.UI.Disable();
            inputAction.Dispose();
            
            return true;
        }
    }
}