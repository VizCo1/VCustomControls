using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime.CustomComponents
{
    [UxmlElement]
    public partial class VDropdownCustom : DropdownField
    {
        private const string BaseDropdownClass = "unity-base-dropdown";
        private const bool OneTime = true;
        
        [Header(nameof(VDropdownCustom))]
        
        [UxmlAttribute]
        private string ClassToAdd { get; set; }
        
        public VDropdownCustom() 
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                RegisterCallback<MouseDownEvent>(OnMouseDown);
            }
            else
            {
                RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            }
#else
            RegisterCallback<MouseDownEvent>(OnMouseDown);
#endif
        }

#if UNITY_EDITOR
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (ClassToAdd.Contains(" "))
            {
                Debug.LogError($"{nameof(ClassToAdd)} can't have spaces: {ClassToAdd}");
            }
        }
#endif
        
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
            var baseDropdownElement = panel.visualTree.Q(className: BaseDropdownClass);
            
            baseDropdownElement.AddToClassList(ClassToAdd);
        }
    }
}