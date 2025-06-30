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

            Root.panel.RegisterTooltip(TooltipClass1);
            var tooltip1 = Root.panel.GetTooltip(TooltipClass1);
            
            var tooltipExample1 = Root.Q("TooltipExample1");
            var tooltipExample2 = Root.Q("TooltipExample2");
            
            tooltipExample1.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Left));
            tooltipExample2.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Right));
            
            Root.panel.RegisterTooltip(TooltipClass2);
            var tooltip2 = Root.panel.GetTooltip(TooltipClass2);
            
            var tooltipExample3 = Root.Q("TooltipExample3");
            var tooltipExample4 = Root.Q("TooltipExample4");
            
            tooltipExample3.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Top));
            tooltipExample4.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Bottom));
        }

        protected override void BeforeDestroy()
        {
            Root.panel.UnregisterTooltip(TooltipClass1);
            Root.panel.UnregisterTooltip(TooltipClass2);
        }
    }
}
