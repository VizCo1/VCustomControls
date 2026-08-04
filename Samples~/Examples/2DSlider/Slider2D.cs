using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;

namespace Samples
{
    public class Slider2D : VBaseView<Slider2DElements>
    {
        
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.VSlider2D.RegisterValueChangedCallback(OnSlider2DValueChanged);
        }
        
        protected void OnDestroy()
        {
            Elements.VSlider2D.UnregisterValueChangedCallback(OnSlider2DValueChanged);
        }
        
        private void OnSlider2DValueChanged(ChangeEvent<Vector2> evt)
        {
            Debug.Log(evt.newValue);
        }
    }
}
