using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VCustomComponents
{
    public class ManagerUI : MonoBehaviour
    {
        [SerializeField]
        private ViewContainer _viewContainer;
        
        public ManagerUI Instance { get; private set; }
        
        private Stack<BaseView> _viewStack;

        private void Start()
        {
            PushDocument<LobbyView>();
        }

        public void PushDocument<T>() where T : BaseView
        {
            var view = _viewContainer.GetView<T>();
            _viewStack.Push(Instantiate(view, transform));
        }

        public void PopDocument()
        {
            if (!_viewStack.TryPop(out var view))
                return;
            
            Destroy(view.gameObject);
        }
        
    }
}
