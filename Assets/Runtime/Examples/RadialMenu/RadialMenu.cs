using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class RadialMenu : ViewBase
    {
        private VRadialMenu _radialMenu;

        protected override void Start()
        {
            base.Start();
            
            _radialMenu = Root.Q<VRadialMenu>();
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

        protected override void BeforeDestroy()
        {
            _radialMenu.OnSlotClicked -= OnSlotClicked;
        }
    }
}
