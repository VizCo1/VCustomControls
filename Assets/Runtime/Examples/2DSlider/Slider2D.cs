using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class Slider2D : ViewBase
    {
        private VSlider2D _slider2D;

        protected override void Start()
        {
            base.Start();
            
            _slider2D = Root.Q<VSlider2D>();

            _slider2D.RegisterValueChangedCallback(OnSlider2DValueChanged);
        }

        protected override void OnDestroy()
        {
            _slider2D.UnregisterValueChangedCallback(OnSlider2DValueChanged);
            
            base.OnDestroy();
        }
        
        private void OnSlider2DValueChanged(ChangeEvent<Vector2> evt)
        {
            Debug.Log(evt.newValue);
        }
    }
}
