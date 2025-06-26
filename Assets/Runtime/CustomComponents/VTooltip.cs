using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VTooltip : Label
    {
        private const bool DoOneTime = true;
        
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
            this.SetDisplay(false);
            
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
            this.SetDisplay(true);
            
            text = target.tooltip;

            schedule
                .Execute(() =>
                {
                    var position = GetTooltipPosition(target, tooltipPosition);
                    
                    style.left = position.x;
                    style.top = position.y;
                })
                .Until(() => DoOneTime);
        }
        
        public void Hide()
        {
            this.SetDisplay(false);
        }
        
        private Vector2 GetTooltipPosition(VisualElement target, VTooltipPosition tooltipPosition)
        {
            return tooltipPosition switch
            {
                VTooltipPosition.Top => new Vector2(target.worldBound.center.x - resolvedStyle.width * 0.5f, target.worldBound.yMin - resolvedStyle.height),
                VTooltipPosition.Right => new Vector2(target.worldBound.xMax, target.worldBound.center.y - resolvedStyle.height * 0.5f),
                VTooltipPosition.Bottom => new Vector2(target.worldBound.center.x - resolvedStyle.width * 0.5f, target.worldBound.yMax),
                VTooltipPosition.Left => new Vector2(target.worldBound.xMin - resolvedStyle.width, target.worldBound.center.y - resolvedStyle.height * 0.5f),
                _ => throw new ArgumentOutOfRangeException(nameof(tooltipPosition), tooltipPosition, null)
            };
        }
    }
}