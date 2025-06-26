using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class VTooltipManipulator : Manipulator
    {
        private readonly VTooltip _vTooltip;
        private readonly VTooltipPosition _vTooltipPosition;
        
        public VTooltipManipulator(VTooltip tooltip, VTooltipPosition position)
        {
            _vTooltip = tooltip;
            _vTooltipPosition = position;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.RegisterCallback<MouseOutEvent>(OnMouseOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.UnregisterCallback<MouseOutEvent>(OnMouseOut);
        }
        
        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _vTooltip.Show(target, _vTooltipPosition);
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            _vTooltip.Hide();
        }
    }
}
