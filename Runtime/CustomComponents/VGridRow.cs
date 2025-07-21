using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
    public sealed class VGridRow : VisualElement
    {
        public static readonly string VGridRowClass = "grid-row";
        public static readonly string VLastGridRowClass = VGridRowClass + "-last";
			
        private readonly Runtime.VGridListView _gridView;
        private readonly List<VisualElement> _gridElements = new();
		
        public VGridRow(Runtime.VGridListView gridListView)
        {
            _gridView = gridListView;
				
            AddToClassList(VGridRowClass);
        }
			
        public void BindToGridRowData(VGridRowData rowData)
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