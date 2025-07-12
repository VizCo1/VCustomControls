using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public static class VTooltipManager
    {
        private static readonly Dictionary<IPanel, Dictionary<string, VTooltip>> TooltipsInPanel = new();
        
        public static bool TryGetTooltip(this IPanel panel, string tooltipClass, out VTooltip tooltip)
        {
            tooltip = null;
            
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");

            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips))
            {
                Debug.LogWarning($"No Tooltips found for {panel.visualTree.name}");
                return false;
            }

            if (!tooltips.TryGetValue(tooltipClass, out tooltip))
            {
                Debug.LogWarning($"No tooltip found with class: {tooltipClass}");
                return false;
            }
            
            return true;
        }
        
        public static bool TryRegisterTooltip(this IPanel panel, string tooltipClass, out VTooltip tooltip)
        {
            tooltip = null;
            
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");
            
            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips))
            {
                tooltips = new Dictionary<string, VTooltip>();
                TooltipsInPanel.TryAdd(panel, tooltips);
            }
            
            tooltip = new VTooltip(tooltipClass);

            if (!tooltips.TryAdd(tooltipClass, tooltip))
            {
                Debug.LogError($"Duplicated tooltip class: {tooltipClass}");
                return false;
            }
            
            panel.visualTree.Add(tooltip);
            
            return true;
        }
        
        public static bool TryUnregisterTooltip(this IPanel panel, string tooltipClass)
        {
            if (tooltipClass == VTooltip.TooltipClass)
                throw new Exception($"The tooltip class can't be equal to {VTooltip.TooltipClass}");

            if (!TooltipsInPanel.TryGetValue(panel, out var tooltips))
            {
                Debug.LogWarning($"The panel {panel.visualTree.name} doesn't have tooltips");
                return false;
            }

            if (!tooltips.Remove(tooltipClass, out var tooltip))
            {
                Debug.LogWarning($"The tooltip {tooltipClass} was not found");
                return false;
            }
            
            if (tooltips.Count == 0)
            {
                TooltipsInPanel.Remove(panel);
            }
            
            panel.visualTree.Remove(tooltip);

            return true;
        }
    }
}
