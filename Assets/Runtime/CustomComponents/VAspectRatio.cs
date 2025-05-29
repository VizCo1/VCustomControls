using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VAspectRatio : VisualElement
    {
        private static readonly BindingId RatioWidthProperty = (BindingId) nameof(RatioWidth);
        private static readonly BindingId RatioHeightProperty = (BindingId) nameof(RatioHeight);
        
        private const float Limit = 1.65f;
        private const string VAspectRatioClass = "aspect-ratio";
        
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
                UpdateAspect();
                
                if (panel == null) 
                    return;
                
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
                UpdateAspect();
                
                if (panel == null) 
                    return;
                
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
                UpdateAspect();
            }
        }
    
        private int _ratioWidth = 16;
        private int _ratioHeight = 9;
        private bool _applyOnlyToWideScreen;
        
        public VAspectRatio()
        {
            AddToClassList(VAspectRatioClass);
            
            RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
            RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
        }
        
        static void UpdateAspectAfterEvent(EventBase evt)
        {
            var element = evt.target as VAspectRatio;
            element?.UpdateAspect();
        }
        
        private void ClearPadding()
        {
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingBottom = 0;
            style.paddingTop = 0;
        }
            
        private void UpdateAspect()
        {
            var designRatio = (float)RatioWidth / RatioHeight;
            var currentRatio = resolvedStyle.width / resolvedStyle.height;
            var difference = currentRatio - designRatio;
            
            if (ApplyOnlyToWideScreen && currentRatio < Limit)
            {
                ClearPadding();
                return;
            }
            
            if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
            {
                ClearPadding();
                Debug.LogError($"[VAspectRatio] Invalid width:{RatioWidth} or height:{RatioHeight}");
                return;
            }
        
            if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
            {
                return;
            }
                
            if (difference > 0.01f)
            {
                var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
                style.paddingLeft = w;
                style.paddingRight = w;
                style.paddingTop = 0;
                style.paddingBottom = 0;
            }
            else if (difference < -0.01f)
            {
                var h = (resolvedStyle.height - (resolvedStyle.width * (1/designRatio))) * 0.5f;
                style.paddingLeft= 0;
                style.paddingRight = 0;
                style.paddingTop = h;
                style.paddingBottom = h;
            }
            else
            {
                ClearPadding();
            }
        }
    }
}