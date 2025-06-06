using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace VCustomComponents
{
    [UxmlElement]
    public partial class VAnimatedSprite : VisualElement, INotifyValueChanged<bool>
    {
        private static readonly BindingId ValueProperty = (BindingId) nameof(value);
        
        public static readonly string VAnimatedSpriteClass = "animated-sprite"; 
        
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
                PlayAnimation();
            }
        }

        [UxmlAttribute] 
        public int Loops  { get; set; } = -1;
        
        [UxmlAttribute]
        public int CurrentIndex { get; private set; }

        [UxmlAttribute]
        public Sprite[] Sprites
        {
            get => _sprites;
            set
            {
                StopAnimation();
                
                _sprites = value;
                
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
        private Sprite[] _sprites;
        
        private IVisualElementScheduledItem _scheduledItem;
        
        public VAnimatedSprite() 
        {
            AddToClassList(VAnimatedSpriteClass);
            
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
        }
    
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            style.backgroundImage = new StyleBackground(Sprites[CurrentIndex]);
            
            _completedLoops = 0;
        }
        
        private void PlayAnimation()
        {
            _scheduledItem?.Pause();

            if (ShouldStopAnimation())
                return;
            
            var intervalMs = 1000 / FrameRate; 
            
            _scheduledItem = schedule
                .Execute(() =>
                {
                    style.backgroundImage = new StyleBackground(Sprites[CurrentIndex]);
                    CurrentIndex++;

                    if (CurrentIndex >= Sprites.Length)
                    {
                        CurrentIndex = 0;
                        _completedLoops++;
                    }
                })
                .Until(ShouldStopAnimation)
                .Every(intervalMs);
        }

        private bool ShouldStopAnimation()
        {
            if (Loops == -1)
                return false;

            if (CurrentIndex >= Sprites.Length || CurrentIndex < 0)
                return true;
            
            return Loops == 0 || _completedLoops >= Loops;
        }

        private void StopAnimation()
        {
            _scheduledItem?.Pause();
        }

        public void ResetAnimationIndex(int newAnimationIndex = 0)
        {
            CurrentIndex = newAnimationIndex;
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
    }
}