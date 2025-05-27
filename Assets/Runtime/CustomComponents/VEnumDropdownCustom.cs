using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime.CustomComponents
{
    [UxmlElement]
    public partial class VEnumDropdownCustom : EnumField
    {
        private const bool OneTime = true;
        
        [Header(nameof(VEnumDropdownCustom))]
        
        [UxmlAttribute]
        private string Class { get; set; }
        
        public VEnumDropdownCustom() 
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