using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents
{
	[UxmlElement]
	public partial class VGridListView : VisualElement
	{
		private readonly ListView _listView;
		
		public Func<VisualElement> MakeCell { get; set; }
		public Action<VisualElement, object> BindCell { get; set; }
		
		public VGridListView()
		{
			_listView = new ListView
			{
				makeItem = MakeItem,
				bindItem = BindItem,
				fixedItemHeight = 100,
			};
			
			Add(_listView);
		}

		public void BindToGrid<T>(T[,] grid)
		{
			var height = grid.GetLength(0);

			var rowData = new List<GridRowData>();
			for (var i = 0; i < height; i++)
			{
				var data = new GridRowData(i, grid);
				rowData.Add(data);
			}

			_listView.itemsSource = rowData;
		}
		
		private VisualElement MakeItem()
		{
			return new GridRow(this);
		}

		private void BindItem(VisualElement visualElement, int index)
		{
			var rowData = (GridRowData)_listView.itemsSource[index];
			var gridRow = (GridRow)visualElement;
			gridRow.BindToGridRowData(rowData);
		}
		
		public readonly struct GridRowData
		{
			public readonly int Row;
			public readonly Array Grid;

			public GridRowData(int row, Array grid)
			{
				Row = row;
				Grid = grid;
			}

			public int GetWidth() => Grid.GetLength(1);
		}

		public sealed class GridRow : VisualElement
		{
			public GridRow(VGridListView gridListView)
			{
				_gridView = gridListView;
				style.flexGrow = 1;
				style.flexDirection = FlexDirection.Row;
				style.justifyContent = Justify.SpaceBetween;
			}
			
			private readonly VGridListView _gridView;

			private readonly List<VisualElement> _gridElements = new();
			
			public void BindToGridRowData(GridRowData rowData)
			{
				var width = rowData.GetWidth();
				if (_gridElements.Count < width)
				{
					var dif = width - _gridElements.Count;
					for (var i = 0; i < dif; i++)
					{
						if (_gridView.MakeCell != null)
						{
							var visualElement = _gridView.MakeCell();
							_gridElements.Add(visualElement);
						}
					}
				}

				Clear();
				
				for (var i = 0; i < width; i++)
				{
					var visualElement = _gridElements[i];
					Add(visualElement);
					
					var data = rowData.Grid.GetValue(rowData.Row, i);
					if (_gridView.BindCell != null)
					{
						_gridView.BindCell(visualElement, data);
					}
				}
			}
		}
	}
}