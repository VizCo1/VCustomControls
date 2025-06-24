using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class VTooltipManipulator : Manipulator
    {
        
        private readonly VTooltip _vTooltip;
        
        public VTooltipManipulator(VTooltip tooltip)
        {
            _vTooltip = tooltip;
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
            _vTooltip.text = target.tooltip;
            
            _vTooltip.style.left = target.worldBound.center.x;
            _vTooltip.style.top = target.worldBound.yMin;
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            _vTooltip.SetDisplay(false);
        }
    }
}
