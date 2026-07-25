using System;
using UnityEngine.UIElements;
using UserInterfaceGenerator;

namespace VCustomComponents.Runtime
{
    public class AnimatedSprite : VBaseView<AnimatedSpriteElements>
    {
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.AnimatedSpriteLeftButton.ExamplesButton.clicked += LeftExamplesButtonOnClicked;
            Elements.AnimatedSpriteCenterButton.ExamplesButton.clicked += CenterExamplesButtonOnClicked;
            Elements.AnimatedSpriteRightButton.ExamplesButton.clicked += RightExamplesButtonOnClicked;
        }

        protected void OnDestroy()
        {
            Elements.AnimatedSpriteLeftButton.ExamplesButton.clicked -= LeftExamplesButtonOnClicked;
            Elements.AnimatedSpriteCenterButton.ExamplesButton.clicked -= CenterExamplesButtonOnClicked;
            Elements.AnimatedSpriteRightButton.ExamplesButton.clicked -= RightExamplesButtonOnClicked;
        }

        private void LeftExamplesButtonOnClicked()
        {
            Elements.VAnimatedSpriteLeft.value = !Elements.VAnimatedSpriteLeft.value;
        }
        
        private void CenterExamplesButtonOnClicked()
        {
            Elements.VAnimatedSpriteMiddle.value = !Elements.VAnimatedSpriteMiddle.value;
        }
        
        private void RightExamplesButtonOnClicked()
        {
            Elements.VAnimatedSpriteRight.value = !Elements.VAnimatedSpriteRight.value;
        }
    }
}
