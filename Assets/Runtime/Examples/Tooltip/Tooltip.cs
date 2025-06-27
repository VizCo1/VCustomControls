using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class Tooltip : ViewBase
    {
        private VTooltip _tooltip;
        
        protected override void Start()
        {
            base.Start();
            
            Root.panel.TryGetTooltipFromPanel("Tooltip1", out _tooltip);

            var tooltipExample1 = Root.Q("TooltipExample1");
            var tooltipExample2 = Root.Q("TooltipExample2");
            
            tooltipExample1.AddManipulator(new VTooltipManipulator(_tooltip, VTooltipPosition.Left));
            tooltipExample2.AddManipulator(new VTooltipManipulator(_tooltip, VTooltipPosition.Right));
        }
    }
}
