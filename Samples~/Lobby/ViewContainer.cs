using System;
using System.Collections.Generic;
using UnityEngine;
using VCustomComponents.Runtime;

namespace Samples
{
    [CreateAssetMenu(fileName = "ViewContainer", menuName = "Scriptable Objects/ViewContainer")]
    public class ViewContainer : ScriptableObject
    {
        [field: SerializeField]
        public ViewBase[] Views { get; private set; }
        
        private readonly Dictionary<Type, ViewBase> _viewDictionary = new();
        
        private void OnEnable()
        {
            _viewDictionary.Clear();
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
