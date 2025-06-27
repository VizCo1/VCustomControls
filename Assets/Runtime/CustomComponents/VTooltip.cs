using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VTooltip : Label
    {
        private const bool DoOneTime = true;

        public static readonly string TooltipClass = "tooltip";

        [Header(nameof(VTooltip))]
        [UxmlAttribute, Tooltip("Fade duration in seconds, if set to -1 there will be no fade")]
        private float FadeTimeInSeconds { get; set; } = -1f;

        public VTooltip() 
        {
            style.position = Position.Absolute;
            AddToClassList(TooltipClass);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            style.opacity = 0;
        }

        public void Show(VisualElement target, VTooltipPosition tooltipPosition)
        {
            FadeIn();
            
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
            FadeOut();
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

        private void FadeIn()
        {
            style.opacity = 0;
            
            schedule
                .Execute(() =>
                { 
                    style.opacity = resolvedStyle.opacity + 0.05f;  
                })
                .Until(() => resolvedStyle.opacity >= 1f);
        }

        private void FadeOut()
        {
            style.opacity = 1;
            
            schedule
                .Execute(() =>
                {
                    style.opacity = resolvedStyle.opacity - 0.05f; 
                })
                .Until(() => resolvedStyle.opacity <= 0f);
        }
    }
}