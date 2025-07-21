using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public class VTooltipManipulator : Manipulator
    {
        private readonly Runtime.VTooltip _vTooltip;
        private readonly VTooltipPosition _vTooltipPosition;
        
        public VTooltipManipulator(Runtime.VTooltip tooltip, VTooltipPosition position)
        {
            _vTooltip = tooltip;
            _vTooltipPosition = position;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }
        
        private void OnPointerEnter(PointerEnterEvent evt)
        {
            _vTooltip.Show(target, _vTooltipPosition);
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _vTooltip.Hide();
        }
        
        private void OnPointerDown(PointerDownEvent evt)
        {
            if (!_vTooltip.TryHide())
                return;
            
            target.RegisterCallbackOnce<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            _vTooltip.Show(target, _vTooltipPosition);
        }
    }
}
