using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VCustomEventPanel : VisualElement
    {
        public static readonly string VCustomEventPanelClass = "custom-event-panel";
        private static VInputActionUI _inputActionAsset;

        private VisualElement _aimEventTarget;
        
        public VCustomEventPanel() 
        {
            AddToClassList(VCustomEventPanelClass);
            
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
#if UNITY_EDITOR
            if (panel.contextType == ContextType.Editor || !Application.isPlaying)
                return;
#endif
            
            if (_inputActionAsset != null)
                return;
            
            _inputActionAsset = new VInputActionUI();
            _inputActionAsset.UI.Enable();
            _inputActionAsset.UI.Aim.performed += AimOnPerformed;
            _inputActionAsset.UI.PostSubmit.performed += PostSubmitOnPerformed;
            _inputActionAsset.UI.PostCancel.performed += PostCancelOnPerformed;
        }

        private void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
#if UNITY_EDITOR
            if (panel.contextType == ContextType.Editor || !Application.isPlaying)
                return;
#endif
            
            _inputActionAsset.UI.Aim.performed -= AimOnPerformed;
            _inputActionAsset.UI.PostSubmit.performed -= PostSubmitOnPerformed;
            _inputActionAsset.UI.PostCancel.performed -= PostCancelOnPerformed;
            _inputActionAsset.UI.Disable();
            _inputActionAsset.Dispose();
            
            _inputActionAsset = null;
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
        
        private void PostSubmitOnPerformed(InputAction.CallbackContext obj)
        {
            var postSubmitTarget = focusController.focusedElement;
            
            if (!IsTargetValid(postSubmitTarget, VCustomEventType.PostSubmitEvent))
                return;
                
            if (panel == null) 
                return;
            
            using var pooled = NavigationPostSubmitEvent.GetPooled();
            
            pooled.target = postSubmitTarget;
            SendEvent(pooled);
        }
        
        private void PostCancelOnPerformed(InputAction.CallbackContext obj)
        {
            var postSubmitTarget = focusController.focusedElement;
            
            if (!IsTargetValid(postSubmitTarget, VCustomEventType.PostCancelEvent))
                return;
                
            if (panel == null) 
                return;
            
            using var pooled = NavigationPostCancelEvent.GetPooled();
            
            pooled.target = postSubmitTarget;
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