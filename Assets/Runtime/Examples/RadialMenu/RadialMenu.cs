using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class RadialMenu : ViewBase
    {
        private VRadialMenu _radialMenu;
        private Button _focusButton;

        protected override void Start()
        {
            base.Start();
            
            _radialMenu = Root.Q<VRadialMenu>();
            _radialMenu.OnSlotClicked += OnSlotClicked;
            
            _focusButton = Root.Q<VisualElement>("FocusButton").Q<Button>();
            
            _focusButton.clicked += OnFocusButtonClicked;
        }

        private void OnFocusButtonClicked()
        {
            _radialMenu.Focus();
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
            _focusButton.clicked -= OnFocusButtonClicked;
        }
    }
}
