using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime.CustomComponents
{
    [UxmlElement]
    public partial class VDropdownCustom : DropdownField
    {
        private const bool OneTime = true;
        
        [Header(nameof(VDropdownCustom))]
        
        [UxmlAttribute]
        private string Class { get; set; }
        
        public VDropdownCustom() 
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0) 
                return;

            schedule
                .Execute(DelayedInit)
                .Until(() => OneTime);
        }

        private void DelayedInit()
        {
            var baseDropdownElement = panel.visualTree.Q(className: "Dropdown");
            
            baseDropdownElement.AddToClassList(Class);
        }
    }
}