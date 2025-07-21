using System;
using System.Collections.Generic;
using UnityEngine;

namespace VCustomComponents.Runtime
{
    [CreateAssetMenu(fileName = "ViewContainer", menuName = "Scriptable Objects/ViewContainer")]
    public class ViewContainer : ScriptableObject
    {
        [field: SerializeField]
        public ViewBase[] Views { get; private set; }
        
        public int NumberOfViews => Views.Length;
        
        private Dictionary<Type, ViewBase> _viewDictionary;
        
        private void OnEnable()
        {
            _viewDictionary = new Dictionary<Type, ViewBase>();
            foreach (var view in Views)
            {
                _viewDictionary.Add(view.GetType(), view);
            }
        }
        
        public ViewBase GetView<T>() where T : ViewBase
        {
            if (!_viewDictionary.TryGetValue(typeof(T), out var view))
                throw new Exception($"No view found for type {typeof(T)}");
            
            return view;
        }
        
    }
}
