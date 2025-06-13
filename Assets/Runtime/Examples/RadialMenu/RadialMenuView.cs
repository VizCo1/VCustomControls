using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class RadialMenuView : BaseView
    {
        private VRadialMenu _radialMenu;

        private void Start()
        {
            _radialMenu = _document.rootVisualElement.Q<VRadialMenu>();
            _radialMenu.OnSlotClicked += OnSlotClicked;
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
        
        private void OnDestroy() 
        {
            _radialMenu.OnSlotClicked -= OnSlotClicked;
        }
    }
}
