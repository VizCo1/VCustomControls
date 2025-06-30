using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VScrollableLabel : Label
    {
        [Header(nameof(VScrollableLabel))]
        
        [UxmlAttribute]
        private int Example { get; set; }

        public VScrollableLabel() 
        {
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