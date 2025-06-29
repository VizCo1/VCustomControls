using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public static class VTooltipExtensions
    {
        private static readonly Dictionary<string, VTooltip> Tooltips = new();
        
        public static VTooltip GetTooltipFromPanel(this IPanel panel, string tooltipName)
        {
            var root = panel.visualTree;
            
            if (!root.TryGetVisualElement<VTooltip>(tooltipName, null, out var tooltip))
            {
                tooltip = RegisterTooltipToPanel(tooltipName, panel);
            }
            
            return tooltip;
        }
        
        public static VTooltip RegisterTooltipToPanel(string tooltipName, IPanel panel)
        {
            var tooltip = new VTooltip(tooltipName);
            Tooltips.Add(tooltipName, tooltip);
            
            panel.visualTree.Add(tooltip);
            
            return tooltip;
        }
        
        public static void UnregisterTooltipFromPanel(string tooltipName, IPanel panel)
        {
            Tooltips.TryGetValue(tooltipName, out var tooltip);
            
            Tooltips.Remove(tooltipName);
            
            panel.visualTree.Remove(tooltip);
        }
    }
}
