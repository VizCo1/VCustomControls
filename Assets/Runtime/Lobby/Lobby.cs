using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public class Lobby : ViewBase
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
            
            _libraryGridListView = Root.Q<VGridListView>();
            _libraryGridListView.BindCell = BindCell;
            
            var rows = (int)Mathf.Round(_viewContainer.NumberOfViews / (float)_columns) + _viewContainer.NumberOfViews % _columns;
            
            _names = new string[rows * _columns];
            
            var cellIndex = 0;
            var grid = new int[rows, _columns];
            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (cellIndex >= _viewContainer.NumberOfViews)
                    {
                        grid[y, x] = -1;
                        continue;
                    } 
                    
                    _names[cellIndex] = _viewContainer.Views[cellIndex].name[0..^4];
                    grid[y, x] = cellIndex++;
                }
            }

            _libraryGridListView.BindToGrid(grid);
        }

        private void BindCell(VisualElement visualElement, int index)
        {
            if (index == -1)
                return;
            
            var button = visualElement.Q<Button>();
            
            button.UnregisterCallback<ClickEvent, int>(OnCellClicked);
            button.RegisterCallback<ClickEvent, int>(OnCellClicked, index);
            
            button.text = _names[index];
        }

        private void OnCellClicked(ClickEvent evt, int index)
        {
            UiManager.Instance.PushDocument(index);
        }
    }
}
