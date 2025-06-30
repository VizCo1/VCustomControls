using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public static class VTooltipExtensions
    {
        private static readonly Dictionary<IPanel, Dictionary<string, VTooltip>> TooltipsInPanel = new();
        
        public static VTooltip GetTooltip(this IPanel panel, string tooltipClass)
        {
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");
            
            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips ))
                throw new Exception($"No Tooltips found for {panel.visualTree.name}");
            
            if (!tooltips.TryGetValue(tooltipClass, out var tooltip))
                throw new Exception($"No tooltip found with class: {tooltipClass}");
            
            return tooltip;
        }
        
        public static VTooltip RegisterTooltip(this IPanel panel, string tooltipClass)
        {
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");
            
            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips))
            {
                tooltips = new Dictionary<string, VTooltip>();
                TooltipsInPanel.TryAdd(panel, tooltips);
            }
            
            var tooltip = new VTooltip(tooltipClass);
            
            if (!tooltips.TryAdd(tooltipClass, tooltip))
                throw new Exception($"Duplicated tooltip class: {tooltipClass}");
            
            panel.visualTree.Add(tooltip);
            
            return tooltip;
        }
        
        public static void UnregisterTooltip(this IPanel panel, string tooltipClass)
        {
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");
            
            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips))
                throw new Exception($"The panel {panel.visualTree.name} doesn't have tooltips");
            
            tooltips.Remove(tooltipClass, out var tooltip);
            
            if (tooltips.Count == 0)
            {
                TooltipsInPanel.Remove(panel);
            }
            
            panel.visualTree.Remove(tooltip);
        }
    }
}
