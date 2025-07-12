using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class Tooltip : ViewBase
    {
        private const string TooltipClass1 = "tooltip1";
        private const string TooltipClass2 = "tooltip2";
        
        protected override void Start()
        {
            base.Start();

            if (Root.panel.TryRegisterTooltip(TooltipClass1, out var tooltip1))
            {
                // Not needed because we're registering the tooltip
                // Root.panel.TryGetTooltip(TooltipClass1, out tooltip1);
                var tooltipExample1 = Root.Q("TooltipExample1");
                var tooltipExample2 = Root.Q("TooltipExample2");
                
                tooltipExample1.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Left));
                tooltipExample2.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Right));
            }
            
            if (Root.panel.TryRegisterTooltip(TooltipClass2,  out var tooltip2))
            {
                // Not needed because we're registering the tooltip
                // Root.panel.TryGetTooltip(TooltipClass2, out tooltip2); 
                var tooltipExample3 = Root.Q("TooltipExample3");
                var tooltipExample4 = Root.Q("TooltipExample4");
                
                tooltipExample3.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Top));
                tooltipExample4.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Bottom));
            }
        }

        protected override void BeforeDestroy()
        {
            Root.panel.TryUnregisterTooltip(TooltipClass1);
            Root.panel.TryUnregisterTooltip(TooltipClass2);
        }
    }
}
