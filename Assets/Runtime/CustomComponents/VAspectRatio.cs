using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VAspectRatio : VisualElement
    {
        public static readonly string VAspectRatioClass = "aspect-ratio";
        
        private static readonly BindingId RatioWidthProperty = (BindingId) nameof(RatioWidth);
        private static readonly BindingId RatioHeightProperty = (BindingId) nameof(RatioHeight);
        
        private const float Limit = 1.65f;
        
        [Header(nameof(VAspectRatio))]
        
        [UxmlAttribute, CreateProperty]
        public int RatioWidth
        {
            get => _ratioWidth;
            set
            {
                if (value == _ratioWidth)
                    return;
                
                _ratioWidth = value;
                UpdateAspectRatio();
                
                NotifyPropertyChanged(in RatioWidthProperty);
            }
        }
        
        [UxmlAttribute, CreateProperty]
        public int RatioHeight
        {
            get => _ratioHeight;
            set
            {
                if (value == _ratioHeight)
                    return;
                
                _ratioHeight = value;
                UpdateAspectRatio();
                
                NotifyPropertyChanged(in RatioHeightProperty);
            }
        }

        [UxmlAttribute]
        public bool ApplyOnlyToWideScreen
        {
            get => _applyOnlyToWideScreen;
            set
            {
                _applyOnlyToWideScreen = value;
                UpdateAspectRatio();
            }
        }
    
        private int _ratioWidth = 16;
        private int _ratioHeight = 9;
        private bool _applyOnlyToWideScreen;
        
        public VAspectRatio()
        {
            AddToClassList(VAspectRatioClass);
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == GeometryChangedEvent.TypeId())
            {
                UpdateAspectRatio();
            }
        }
        
        private void ClearPadding()
        {
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingBottom = 0;
            style.paddingTop = 0;
        }
            
        private void UpdateAspectRatio()
        {
            if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
            {
                Debug.LogError($"Invalid width:{RatioWidth} or height:{RatioHeight}");
            }
            
            var designRatio = (float)RatioWidth / RatioHeight;
            var currentRatio = resolvedStyle.width / resolvedStyle.height;
            var difference = currentRatio - designRatio;
            
            if (ApplyOnlyToWideScreen && currentRatio < Limit)
            {
                ClearPadding();
                return;
            }
                
            if (difference > 0.01f)
            {
                var width = (resolvedStyle.width - resolvedStyle.height * designRatio) * 0.5f;
                style.paddingLeft = width;
                style.paddingRight = width;
                style.paddingTop = 0;
                style.paddingBottom = 0;
            }
            else if (difference < -0.01f)
            {
                var height = (resolvedStyle.height - resolvedStyle.width * (1 / designRatio)) * 0.5f;
                style.paddingLeft= 0;
                style.paddingRight = 0;
                style.paddingTop = height;
                style.paddingBottom = height;
            }
            else
            {
                ClearPadding();
            }
        }
    }
}