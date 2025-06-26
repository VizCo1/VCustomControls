using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class Tooltip : ViewBase
    {
        private VTooltip _tooltip;
        

        protected override void Start()
        {
            base.Start();
            
            _tooltip = Root.Q<VTooltip>();

            var tooltipExample1 = Root.Q("TooltipExample1");
            var tooltipExample2 = Root.Q("TooltipExample2");
            
            tooltipExample1.AddManipulator(new VTooltipManipulator(_tooltip, VTooltipPosition.Top));
            tooltipExample2.AddManipulator(new VTooltipManipulator(_tooltip, VTooltipPosition.Bottom));
        }
    }
}
