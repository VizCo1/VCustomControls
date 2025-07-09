using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VEnumDropdownCustom : DropdownField
    {
        private const string BaseDropdownClass = "unity-base-dropdown";
        private const bool OneTime = true;
        
        [Header(nameof(VEnumDropdownCustom))]
        
        [UxmlAttribute]
        private string ClassToAdd { get; set; }
        
        [UxmlAttribute]
        private string ScrollClassToAdd { get; set; }
        
        public VEnumDropdownCustom() 
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }
        
#if UNITY_EDITOR
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);

            if (evt.eventTypeId == AttachToPanelEvent.TypeId())
            {
                OnAttachedToPanel();
            }
        }
        
        private void OnAttachedToPanel()
        {
            if (ClassToAdd.Contains(" "))
            {
                Debug.LogError($"{nameof(ClassToAdd)} can't have spaces: {ClassToAdd}");
            }

            if (ScrollClassToAdd.Contains(" "))
            {
                Debug.LogError($"{nameof(ScrollClassToAdd)} can't have spaces: {ScrollClassToAdd}");
            }
        }
#endif
        
        private void OnPointerDown(PointerDownEvent evt)
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
            
            if (string.IsNullOrEmpty(ScrollClassToAdd))
                return;

            var scrollView = baseDropdownElement.Q<ScrollView>();
            
            scrollView.AddToClassList(ScrollClassToAdd);
        }
    }
}