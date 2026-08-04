using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;

namespace Samples
{
    public class RadialMenu : VBaseView<RadialMenuElements>
    {
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.VRadialMenu.OnSlotClicked += OnSlotClicked;
        }
        
        private void OnSlotClicked(int index)
        {
            switch (index)
            {
                case 0:  
                    Debug.Log("A");
                    break;
                case 1:
                    Debug.Log("B");
                    break;
                case 2:
                    Debug.Log("C");
                    break;
                case 3:
                    Debug.Log("D");
                    break;
            }
        }
        
        protected void OnDestroy()
        {
            Elements.VRadialMenu.OnSlotClicked -= OnSlotClicked;
        }
    }
}
