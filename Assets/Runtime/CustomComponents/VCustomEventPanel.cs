using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VCustomEventPanel : VisualElement
    {
        private static VInputActionUI _inputActionAsset;

        private VisualElement _aimEventTarget;
        
        public VCustomEventPanel() 
        {
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            Debug.Log("Attached from Panel");
            if (_inputActionAsset != null)
                return;
            
            _inputActionAsset = new VInputActionUI();
            _inputActionAsset.UI.Enable();
            
            _inputActionAsset.UI.Aim.performed += AimOnPerformed;
        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            Debug.Log("Detached from Panel");
            _inputActionAsset.UI.Aim.performed -= AimOnPerformed;
            _inputActionAsset.UI.Disable();
        }
        
        private void AimOnPerformed(InputAction.CallbackContext ctx)
        {
            var aimEventTarget = focusController.focusedElement;

            if (!IsTargetValid(aimEventTarget, VCustomEventType.AimEvent))
                return;
                
            if (panel == null) 
                return;
            
            var aimVector = ctx.ReadValue<Vector2>();
            
            using var pooled = VAimEvent.GetPooled(aimVector);
            
            pooled.target = aimEventTarget;
            SendEvent(pooled);
        }

        private bool IsTargetValid(Focusable element, VCustomEventType expectedEventType)
        {
            if (element is not IVHasCustomEvent vHasCustomEvent)
                return false;
            
            return vHasCustomEvent.CustomEvent.HasFlag(expectedEventType);
        }
    }
}