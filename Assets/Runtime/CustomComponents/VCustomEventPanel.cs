using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
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
            if (_inputActionAsset == null)
            {
                _inputActionAsset = new VInputActionUI();
                _inputActionAsset.UI.Enable();
            }
            
            _inputActionAsset.UI.Aim.performed += AimOnPerformed;
            
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            
        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            _inputActionAsset.UI.Disable();
        }
        
        private void AimOnPerformed(InputAction.CallbackContext ctx)
        {
            var aimEventTarget = focusController.focusedElement;
            
            Debug.Log(aimEventTarget);

            if (aimEventTarget is not IVHasCustomEvent)
                return;
                
            if (panel == null) 
                return;
            
            var aimVector = ctx.ReadValue<Vector2>();
            
            using var pooled = VAimEvent.GetPooled(aimVector);
            
            pooled.target = aimEventTarget;
            SendEvent(pooled);
        }
    }
}