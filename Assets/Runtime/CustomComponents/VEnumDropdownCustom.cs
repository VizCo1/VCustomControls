using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VEnumDropdownCustom : EnumField
    {
        private const string BaseDropdownClass = "unity-base-dropdown";
        private const bool OneTime = true;
        
        [Header(nameof(VEnumDropdownCustom))]
        
        [UxmlAttribute]
        private string ClassToAdd { get; set; }
        
        public VEnumDropdownCustom() 
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
            
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
#endif
            RegisterCallback<MouseDownEvent>(OnMouseDown);
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
            if (!panel.visualTree.TryGetVisualElement(null, BaseDropdownClass, out var baseDropdownElement))
                return;
            
            baseDropdownElement.AddToClassList(ClassToAdd);
        }
    }
}