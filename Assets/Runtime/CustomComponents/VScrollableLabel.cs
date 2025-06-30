using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollableLabel : VisualElement
    {
        public static readonly string ScrollableLabelClass = "scrollable-label";
        public static readonly string ScrollableLabelContainerClass = ScrollableLabelClass + "-container";
        
        [Header(nameof(VScrollableLabel))]
        
        private Label Label { get; set; }

        public VScrollableLabel() 
        {
            AddToClassList(ScrollableLabelContainerClass);
            
            Label = new Label("Label");
            Label.AddToClassList(ScrollableLabelClass);
            Add(Label);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
        
        }
    
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
        
        }
    }
}