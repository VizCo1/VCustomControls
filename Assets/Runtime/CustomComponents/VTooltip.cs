using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VTooltip : Label
    {
        private const bool DoOneTime = true;
        private const float FadeRate = 0.05f;

        public static readonly string TooltipClass = "tooltip";

        [Header(nameof(VTooltip))]
        
        [UxmlAttribute]
        private float FadeDuration { get; set; } = 0.1f;
        
        [UxmlAttribute]
        private int TooltipDelayMs { get; set; } = 500;
        
        [UxmlAttribute]
        private int Offset { get; set; } = 5;

        private IVisualElementScheduledItem _scheduledItem;
        private VisualElement _previousTarget;
        private int _intervalMs;
        
        public VTooltip() 
        {
            style.position = Position.Absolute;
            AddToClassList(TooltipClass);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            style.opacity = 0;
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
        }

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            _intervalMs = Mathf.RoundToInt(FadeDuration / (1f / FadeRate) * 1000f);
        }

        public void Show(VisualElement target, VTooltipPosition tooltipPosition, bool canHaveFadeDelay = true)
        {
            panel.visualTree.Add(this);
            
            FadeIn(target, canHaveFadeDelay);
            
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

        public bool TryHide()
        {
            if (resolvedStyle.opacity <= 0f)
                return false;
            
            FadeOut();
            
            return true;
        }
        
        private Vector2 GetTooltipPosition(VisualElement target, VTooltipPosition tooltipPosition)
        {
            return tooltipPosition switch
            {
                VTooltipPosition.Top => new Vector2(target.worldBound.center.x - resolvedStyle.width * 0.5f, target.worldBound.yMin - resolvedStyle.height - Offset),
                VTooltipPosition.Right => new Vector2(target.worldBound.xMax + Offset, target.worldBound.center.y - resolvedStyle.height * 0.5f),
                VTooltipPosition.Bottom => new Vector2(target.worldBound.center.x - resolvedStyle.width * 0.5f, target.worldBound.yMax + Offset),
                VTooltipPosition.Left => new Vector2(target.worldBound.xMin - resolvedStyle.width - Offset, target.worldBound.center.y - resolvedStyle.height * 0.5f),
                _ => throw new ArgumentOutOfRangeException(nameof(tooltipPosition), tooltipPosition, null)
            };
        }

        private void FadeIn(VisualElement target, bool canHaveFadeDelay)
        {
            if (_previousTarget != target)
            {
                style.opacity = 0f;
            }
            
            var startingIn = TooltipDelayMs;

            if (!canHaveFadeDelay || _scheduledItem is { isActive: true } && _previousTarget == target)
            {
                startingIn = 0;
            }
            
            _previousTarget = target;
            
            _scheduledItem?.Pause();
            _scheduledItem = schedule
                .Execute(() =>
                { 
                    style.opacity = resolvedStyle.opacity + FadeRate;  
                })
                .StartingIn(startingIn)
                .Every(_intervalMs)
                .Until(() => resolvedStyle.opacity >= 1f);
        }

        private void FadeOut()
        {
            _scheduledItem?.Pause();
            _scheduledItem = schedule
                .Execute(() =>
                {
                    style.opacity = resolvedStyle.opacity - FadeRate; 
                })
                .Every(_intervalMs)
                .Until(() => resolvedStyle.opacity <= 0f);
        }
    }
}