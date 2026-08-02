using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VCustomComponents.Runtime
{
    [UxmlElement]
    public partial class VRegion : VisualElement
    {
        public static readonly string VRegionClass = "region";
        
        [Header(nameof(VRegion))]
        [UxmlAttribute]
        public GameObject VisualTreeAssetPrefab { get; private set; }
        
        public VRegion()
        {
            AddToClassList(VRegionClass);
            RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
        }
        
        private void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            if (IsEditorOrPreview())
                return;
            
            UnregisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
            VRegionManager.RemoveView(VisualTreeAssetPrefab);
        }
        
        private void AttachedToPanel(AttachToPanelEvent evt)
        {
            if (IsEditorOrPreview())
                return;
            
            UnregisterCallback<AttachToPanelEvent>(AttachedToPanel);
            
            if (!VisualTreeAssetPrefab)
                return;

            // We want to add the view in AttachToPanel, but we have to wait 1 frame
            schedule.Execute(AddViewDelayed);
        }

        private void AddViewDelayed()
        {
            VRegionManager.AddView(VisualTreeAssetPrefab, this);
        }

        private bool IsEditorOrPreview()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying || panel?.contextType == ContextType.Editor)
                return true;
            
            // When selecting a prefab with a PanelRendererComponent it triggers undesired callbacks because it adds temporary elements to the hierarchy
            var currentParent = parent;
            while (currentParent != null)
            {
                var currentParentName = currentParent.name;
                if (currentParentName.Contains("(Clone)") && currentParent.ClassListContains("unity-ui-document__root"))
                    return true;
                
                currentParent = currentParent.parent;
            }
#endif
            return false;
        }
    }
}