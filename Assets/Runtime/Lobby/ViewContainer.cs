using System;
using System.Collections.Generic;
using UnityEngine;

namespace VCustomComponents
{
    [CreateAssetMenu(fileName = "ViewContainer", menuName = "Scriptable Objects/ViewContainer")]
    public class ViewContainer : ScriptableObject
    {
        [SerializeField]
        private BaseView[] _views;
        
        private Dictionary<Type, BaseView> _viewDictionary;
        
        private void OnEnable()
        {
            _viewDictionary = new Dictionary<Type, BaseView>();
            foreach (var view in _views)
            {
                _viewDictionary.Add(view.GetType(), view);
            }
        }
        
        public BaseView GetView<T>() where T : BaseView
        {
            if (!_viewDictionary.TryGetValue(typeof(T), out var view))
                throw new Exception($"No view found for type {typeof(T)}");
            
            return view;
        }
    }
}
