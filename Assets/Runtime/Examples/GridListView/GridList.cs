using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class GridList : ViewBase
    {
        [SerializeField]
        private int _columns;
        
        [SerializeField]
        private int _rows;
        
        private VGridListView _gridListView;

        protected override void Start()
        {
            base.Start();
            
            _gridListView = Root.Q<VGridListView>();
            
            _gridListView.BindCell = BindCell;
            
            var grid = new int[_rows, _columns];
            var cellIndex = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    grid[y, x] = cellIndex++;
                }
            }

            _gridListView.BindToGrid(grid);
        }

        private void BindCell(VisualElement visualElement, int index)
        {
            var button = (Button)visualElement[0];
            
            button.text = index.ToString();
        }
    }
}
