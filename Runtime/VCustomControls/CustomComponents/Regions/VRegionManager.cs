using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace VCustomComponents.Runtime
{ 
    public static class VRegionManager
    {
        private static readonly Dictionary<PanelRenderer, VisualElement> PanelToRegion = new();
        private static readonly Dictionary<VisualTreeAsset, Transform> Views = new();
        
        public static void Init(PanelRenderer[] panelRenderers)
        {
            PanelToRegion.Clear();
            Views.Clear();
            
            foreach (var panelRenderer in panelRenderers)
            {
                Views.Add(panelRenderer.visualTreeAsset, panelRenderer.transform);
            }
        }
        
        public static void AddView(GameObject viewPrefab, VisualElement uxmlContainer)
        {
            if (!viewPrefab.TryGetComponent<PanelRenderer>(out var prefabPanelRenderer))
                throw new Exception("No PanelRenderer attached to " + viewPrefab.name);
            
            if (Views.TryGetValue(prefabPanelRenderer.visualTreeAsset, out _))
                throw new Exception("Tried to add an already active View");
            
            if (!Views.TryGetValue(uxmlContainer.visualTreeAssetSource, out var viewParent))
                throw new Exception("View parent not found, this must not happen");
            
            var panelRenderer = Object.Instantiate(viewPrefab, viewParent).GetComponent<PanelRenderer>();
            Views.Add(panelRenderer.visualTreeAsset, panelRenderer.transform);
            
            PanelToRegion.Add(panelRenderer, uxmlContainer);
            
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
        
        private static void OnUIReload(PanelRenderer panelRenderer, VisualElement viewElement)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
            
            PanelToRegion.TryGetValue(panelRenderer, out var regionElement);

            regionElement!.hierarchy.Add(viewElement);
            viewElement.style.flexGrow = 1; // Do this from a class instead!! TODO
            
            if (regionElement.panel == null) 
                return;
            
            using var pooled = VRegionInitializeEvent.GetPooled(panelRenderer.gameObject);
            
            pooled.target = regionElement;
            regionElement.SendEvent(pooled);
        }

        public static void RemoveView(GameObject prefab)
        {
            if (!prefab.TryGetComponent<PanelRenderer>(out var panelRenderer))
                throw new Exception("No PanelRenderer attached to " + prefab.name);
            
            if (!Views.Remove(panelRenderer.visualTreeAsset, out var viewToRemove))
                throw new Exception("View to remove not found, this must not happen");
            
            PanelToRegion.Remove(panelRenderer);
            
            Object.Destroy(viewToRemove.gameObject);
        }
    }   
}