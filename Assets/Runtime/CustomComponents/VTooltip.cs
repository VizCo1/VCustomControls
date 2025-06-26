using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VTooltip : Label
    {
        [Header(nameof(VTooltip))]
        
        [UxmlAttribute]
        private int Example { get; set; }

        public VTooltip() 
        {
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            this.SetVisibility(false);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
        
        }
    
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
        
        }

        public void Show(VisualElement target, VTooltipPosition tooltipPosition)
        {
            this.SetVisibility(true);
            
            text = target.tooltip;

            var position = GetTooltipPosition(target, tooltipPosition);
            
            style.left = position.x;
            style.top = position.y;
        }
        
        public void Hide()
        {
            this.SetVisibility(false);
        }
        
        private Vector2 GetTooltipPosition(VisualElement target, VTooltipPosition tooltipPosition)
        {
            return tooltipPosition switch
            {
                VTooltipPosition.Top => new Vector2(target.worldBound.center.x - resolvedStyle.width * 0.5f, target.worldBound.yMin - resolvedStyle.height),
                VTooltipPosition.Right => new Vector2(0, 0),
                VTooltipPosition.Bottom => new Vector2(0, 0),
                VTooltipPosition.Left => new Vector2(0, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(tooltipPosition), tooltipPosition, null)
            };
        }
    }
}