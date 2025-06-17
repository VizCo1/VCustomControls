using System.Collections.Generic;
using UnityEngine;

namespace VCustomComponents
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField]
        private ViewContainer _viewContainer;
        
        public static ManagerUI Instance { get; private set; }
        
        private Stack<ViewBase> _viewStack;

        private void Awake()
        {
            if (Instance)
                return;
            
            Instance = this;
            
            DontDestroyOnLoad(this);
            
            _viewStack = new Stack<ViewBase>();
        }

        public void PushDocument<T>() where T : ViewBase
        {
            if (_viewStack.TryPeek(out var previousView))
            {
                previousView.Root.SetDisplay(false);
            }
            
            var view = _viewContainer.GetView<T>();
            
            var baseView = Instantiate(view, transform);
            _viewStack.Push(baseView);
        }

        public void PushDocument(int viewIndex)
        {
            if (_viewStack.TryPeek(out var previousView))
            {
                previousView.Root.SetDisplay(false);
            }
            
            var view = _viewContainer.Views[viewIndex];

            var baseView = Instantiate(view, transform);
            _viewStack.Push(baseView);
        }
        
        public void PopDocument()
        {
            if (!_viewStack.TryPop(out var view))
                return;
            
            Destroy(view.gameObject);
            
            if (_viewStack.TryPeek(out var currentView))
            {
                currentView.Root.SetDisplay(true);
            }
        }
    }
}
