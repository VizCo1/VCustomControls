using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class LobbyView : BaseView
    {
        [SerializeField]
        private ViewContainer _viewContainer;
        
        [SerializeField]
        private int _columns = 4;
        
        private VGridListView _libraryGridListView;
        private string[] _names;

        protected override void Start()
        {
            base.Start();
            
            _libraryGridListView = _document.rootVisualElement.Q<VGridListView>();
            _libraryGridListView.BindCell = BindCell;
            
            var rows = (int)Mathf.Round(_viewContainer.NumberOfViews / (float)_columns);
            
            _names =  new string[rows * _columns];
            
            var cellIndex = 0;
            var grid = new int[rows, _columns];
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    _names[cellIndex] = _viewContainer.Views[cellIndex].name[0..^4];
                    grid[y, x] = cellIndex++;
                }
            }

            _libraryGridListView.BindToGrid(grid);
        }

        private void BindCell(VisualElement visualElement, int index)
        {
            var button = visualElement.Q<Button>();
            
            button.UnregisterCallback<ClickEvent, int>(OnCellClicked);
            button.RegisterCallback<ClickEvent, int>(OnCellClicked, index);
            
            button.text = _names[index];
        }

        private void OnCellClicked(ClickEvent evt, int index)
        {
            ManagerUI.Instance.PushDocument(index);
        }
    }
}
