using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class VBaseView<T> : MonoBehaviour
    {
        protected PanelRenderer PanelRenderer { private set; get; }
        protected VisualElement RootElement { private set; get; }
        protected T Elements { private set; get; }
        
        private int _uiVersion;
        protected virtual void Awake()
        {
            PanelRenderer = GetComponent<PanelRenderer>();
        }
        
        protected virtual void OnEnable()
        {
            PanelRenderer.RegisterUIReloadCallback(OnVersionedUIReload);
        }
        
        protected virtual void OnDisable()
        {
            PanelRenderer.UnregisterUIReloadCallback(OnVersionedUIReload);
        }
        
        private void OnVersionedUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            if (_uiVersion == version)
                return;
            
            Elements = (T)System.Activator.CreateInstance(typeof(T), rootElement, panelRenderer.visualTreeAsset);
            
            _uiVersion = version;
            
            RootElement = rootElement;
            
            OnUIReload(panelRenderer, rootElement);
        }

        protected abstract void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement);

        public void RemoveView()
        {
            RootElement.RemoveFromHierarchy();
            Destroy(gameObject);
        }
    }
}
