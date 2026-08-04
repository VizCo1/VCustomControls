using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator.Runtime;
using VCustomComponents.Runtime;

namespace Samples
{
    public class GridListView : VBaseView<GridListViewElements>
    {
        [SerializeField]
        private int _columns;
        
        [SerializeField]
        private int _rows;
        
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.GridListView.BindCell = BindCell;
            
            var grid = new int[_rows, _columns];
            var cellIndex = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    grid[y, x] = cellIndex++;
                }
            }
            
            Elements.GridListView.BindToGrid(grid);
        }

        private void BindCell(VisualElement visualElement, int index)
        {
            var button = visualElement.Q<Button>();
            
            button.text = index.ToString();
        }
    }
}
