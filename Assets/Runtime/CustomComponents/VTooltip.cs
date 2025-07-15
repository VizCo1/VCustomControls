using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VTooltip : Label
    {
        public static readonly string TooltipClass = "tooltip";
        
        private static readonly CustomStyleProperty<int> FadeDurationMs = new("--fade-duration-ms");
        private static readonly CustomStyleProperty<int> TooltipDelayMs = new("--tooltip-delay-ms");
        private static readonly CustomStyleProperty<int> Offset = new("--offset");
        
        private const bool DoOneTime = true;
        private const float FadeRate = 0.05f;
        
        private int _fadeDurationMs = 100;
        private int _tooltipDelayMs = 500;
        private int _offset = 5;

        private IVisualElementScheduledItem _scheduledItem;
        private VisualElement _previousTarget;
        private int _intervalMs;
        
        public VTooltip() 
        {
            AddToClassList(TooltipClass);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            style.opacity = 0;
        }

        public VTooltip(string tooltipClass) : this()
        {
            if (string.IsNullOrEmpty(tooltipClass))
                throw new ArgumentNullException(nameof(tooltipClass));
            
            AddToClassList(tooltipClass);
            
            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            name = textInfo.ToTitleCase(tooltipClass.Trim('-'));
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            
            if (evt.eventTypeId == CustomStyleResolvedEvent.TypeId())
            {
                OnStylesResolved((CustomStyleResolvedEvent)evt);
            }
        }

        public void Show(VisualElement target, VTooltipPosition tooltipPosition, bool canHaveFadeDelay = true)
        {
            BringToFront();
            
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
        
        private void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(FadeDurationMs, out var fadeDurationMs)) 
            {
                _fadeDurationMs = fadeDurationMs;
            }
            if (evt.customStyle.TryGetValue(TooltipDelayMs, out var tooltipDelayMs)) 
            {
                _tooltipDelayMs = tooltipDelayMs;
            }
            if (evt.customStyle.TryGetValue(Offset, out var offset)) 
            {
                _offset = offset;
            }
            
            _intervalMs = Mathf.RoundToInt(_fadeDurationMs / (1f / FadeRate));
        }
        
        private Vector2 GetTooltipPosition(VisualElement target, VTooltipPosition tooltipPosition)
        {
            return tooltipPosition switch
            {
                VTooltipPosition.Top => new Vector2(
                    target.worldBound.center.x - resolvedStyle.width * 0.5f, 
                    target.worldBound.yMin - resolvedStyle.height - _offset),
                VTooltipPosition.Right => new Vector2(
                    target.worldBound.xMax + _offset, 
                    target.worldBound.center.y - resolvedStyle.height * 0.5f),
                VTooltipPosition.Bottom => new Vector2(
                    target.worldBound.center.x - resolvedStyle.width * 0.5f, 
                    target.worldBound.yMax + _offset),
                VTooltipPosition.Left => new Vector2(
                    target.worldBound.xMin - resolvedStyle.width - _offset, 
                    target.worldBound.center.y - resolvedStyle.height * 0.5f),
                _ => throw new ArgumentOutOfRangeException(nameof(tooltipPosition), tooltipPosition, null)
            };
        }

        private void FadeIn(VisualElement target, bool canHaveFadeDelay)
        {
            if (_previousTarget != target)
            {
                style.opacity = 0f;
            }
            
            var startingIn = _tooltipDelayMs;

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