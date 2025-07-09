using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public sealed class GridRow : VisualElement
    {
        public static readonly string GridRowClass = "grid-row";
        public static readonly string LastGridRowClass = "grid-row-last";
			
        private readonly VGridListView _gridView;
        private readonly List<VisualElement> _gridElements = new();
		
        public GridRow(VGridListView gridListView)
        {
            _gridView = gridListView;
				
            AddToClassList(GridRowClass);
        }
			
        public void BindToGridRowData(GridRowData rowData)
        {
            var width = rowData.GetWidth();
            if (_gridElements.Count < width)
            {
                var dif = width - _gridElements.Count;
                for (var i = 0; i < dif; i++)
                {
                    if (rowData.Grid[rowData.Row, i] == -1)
                        break;
					
                    var visualElement = _gridView.MakeCell();
                    _gridElements.Add(visualElement);
                }
            }

            Clear();
				
            for (var i = 0; i < width; i++)
            {
                if (rowData.Grid[rowData.Row, i] == -1)
                    break;
				
                var visualElement = _gridElements[i];
                Add(visualElement);
					
                var index = rowData.Grid[rowData.Row, i];
                _gridView.BindCell(visualElement, index);
            }
        }
    }
}