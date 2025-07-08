using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VAnimatedSprite : VisualElement, INotifyValueChanged<bool>
    {
        public static readonly string VAnimatedSpriteClass = "animated-sprite"; 
        
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        [Header(nameof(VAnimatedSprite))]
        
        [UxmlAttribute, CreateProperty]
        public bool value
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;
            
                var previousValue = _value;
                SetValueWithoutNotify(value);
            
                if (panel == null) 
                    return;

                using var pooled = ChangeEvent<bool>.GetPooled(previousValue, _value);
            
                pooled.target = this;
                SendEvent(pooled);
            
                NotifyPropertyChanged(in ValueProperty);
            }
        }

        [UxmlAttribute]
        public int FrameRate
        {
            get => _frameRate;
            set
            {
                _frameRate = value;
                if (_value)
                {
                    PlayAnimation();
                }
            }
        }

        [UxmlAttribute] 
        public int Loops  { get; set; } = -1;
        
        [UxmlAttribute]
        public VSpriteAnimation SpriteAnimation
        {
            get => _spriteAnimation;
            set
            {
                if (value == null)
                    Debug.LogError("Can't set Sprites to null");

                StopAnimation();
                
                _spriteAnimation = value;
                
                ResetAnimationIndex();

                if (_value)
                {
                    PlayAnimation();
                }
                else
                {
                    ResetLoops();
                }
            }
        }
        
        private bool _value;
        private int _frameRate = 24;
        private int _completedLoops;
        private VSpriteAnimation _spriteAnimation;
        
        private int _currentIndex;
        private IVisualElementScheduledItem _scheduledItem;
        
        public VAnimatedSprite() 
        {
            AddToClassList(VAnimatedSpriteClass);
            
            _completedLoops = 0;
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (SpriteAnimation == null || SpriteAnimation.Sprites.Length == 0)
                return;
            
            style.backgroundImage = new StyleBackground(SpriteAnimation.Sprites[_currentIndex]);
        }
        
        public void ResetAnimationIndex(int newAnimationIndex = 0)
        {
            _currentIndex = newAnimationIndex;
        }

        public void ResetLoops()
        {
            _completedLoops = 0;
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            _value = newValue;

            if (_value)
            {
                PlayAnimation();
            }
            else
            {
                StopAnimation();
            }
        }
        
        private void PlayAnimation()
        {
            _scheduledItem?.Pause();

            if (ShouldStopAnimation())
                return;
            
            var intervalMs = Mathf.RoundToInt(1000f / FrameRate); 
            
            _scheduledItem = schedule
                .Execute(() =>
                {
                    style.backgroundImage = new StyleBackground(SpriteAnimation.Sprites[_currentIndex]);
                    _currentIndex++;

                    if (_currentIndex < SpriteAnimation.Sprites.Length) 
                        return;
                    
                    _currentIndex = 0;
                    _completedLoops++;
                })
                .Until(ShouldStopAnimation)
                .Every(intervalMs);
        }

        private bool ShouldStopAnimation()
        {
            if (SpriteAnimation == null || SpriteAnimation.Sprites.Length == 0)
                return true;
            
            if (Loops == -1)
                return false;

            if (_currentIndex >= SpriteAnimation.Sprites.Length || _currentIndex < 0)
                return true;
            
            return Loops == 0 || _completedLoops >= Loops;
        }

        private void StopAnimation()
        {
            _scheduledItem?.Pause();
        }
    }
}