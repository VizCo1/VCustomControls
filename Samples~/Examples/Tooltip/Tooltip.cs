using UnityEngine.UIElements;
using UserInterfaceGenerator;

namespace VCustomComponents.Runtime
{
    public class Tooltip : VBaseView<TooltipElements>
    {
        private const string TooltipClass1 = "tooltip1";
        private const string TooltipClass2 = "tooltip2";
        
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            if (RootElement.panel.TryRegisterTooltip(TooltipClass1, out var tooltip1))
            {
                // Not needed because we're registering the tooltip
                // Root.panel.TryGetTooltip(TooltipClass1, out tooltip1);
                
                Elements.TooltipExample1.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Left));
                Elements.TooltipExample2.AddManipulator(new VTooltipManipulator(tooltip1, VTooltipPosition.Right));
            }
            
            if (RootElement.panel.TryRegisterTooltip(TooltipClass2,  out var tooltip2))
            {
                // Not needed because we're registering the tooltip
                // Root.panel.TryGetTooltip(TooltipClass2, out tooltip2); 
                
                Elements.TooltipExample3.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Top));
                Elements.TooltipExample4.AddManipulator(new VTooltipManipulator(tooltip2, VTooltipPosition.Bottom));
            } 
        }

        private void OnDestroy()
        {
            RootElement.panel?.TryUnregisterTooltip(TooltipClass1);
            RootElement.panel?.TryUnregisterTooltip(TooltipClass2);
        }
    }
}
