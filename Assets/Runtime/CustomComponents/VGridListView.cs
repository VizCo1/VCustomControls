using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VCustomComponents
{
	[UxmlElement]
	public partial class VGridListView : VisualElement
	{
		public static readonly string VGridListViewClass = "grid-list-view";
		
		private readonly ListView _listView;
		
		public Action<VisualElement, object> BindCell { get; set; }
		
		public Func<VisualElement> MakeCell { get; private set; }
		
		[UxmlAttribute]
		private int FixedItemHeight { get; set; } = 22;
		
		[UxmlAttribute]
		private CollectionVirtualizationMethod VirtualizationMethod { get; set; }
		
		[UxmlAttribute]
		private bool ShowBorder { get; set; }
		
		[UxmlAttribute]
		private SelectionType SelectionType { get; set; } =  SelectionType.Single;
		
		[UxmlAttribute]
		private AlternatingRowBackground ShowAlternatingRowBackgrounds { get; set; }
		
		[UxmlAttribute]
		private bool Reorderable { get; set; }
		
		[UxmlAttribute]
		private bool HorizontalScrolling { get; set; }
		
		[UxmlAttribute]
		private bool ShowFoldoutHeader { get; set; }
		
		[UxmlAttribute]
		private string HeaderTitle { get; set; }
		
		[UxmlAttribute]
		private bool ShowAddRemoveFooter { get; set; }
		
		[UxmlAttribute]
		private bool AllowAdd { get; set; } = true;
		
		[UxmlAttribute]
		private bool AllowRemove { get; set; } = true;
		
		[UxmlAttribute]
		private ListViewReorderMode ReorderMode { get; set; }
		
		[UxmlAttribute]
		private bool ShowBoundCollectionSize { get; set; } = true;
		
		[UxmlAttribute]
		private BindingSourceSelectionMode BindingSourceSelectionMode { get; set; }
		
		[UxmlAttribute]
		private VisualTreeAsset ItemTemplate { get; set; }
		
		public VGridListView()
		{
			AddToClassList(VGridListViewClass);
			
			_listView = new ListView
			{
				makeItem = MakeItem,
				bindItem = BindItem,
			};
			
			Add(_listView);
			
			RegisterCallbackOnce<AttachToPanelEvent>(AttachedToPanelEvent);
		}

		private void AttachedToPanelEvent(AttachToPanelEvent evt)
		{
			_listView.fixedItemHeight = FixedItemHeight;
			_listView.virtualizationMethod = VirtualizationMethod;
			_listView.showBorder = ShowBorder;
			_listView.selectionType = SelectionType;
			_listView.showAlternatingRowBackgrounds = ShowAlternatingRowBackgrounds;
			_listView.reorderable = Reorderable;
			_listView.horizontalScrollingEnabled = HorizontalScrolling;
			_listView.showFoldoutHeader = ShowFoldoutHeader;
			_listView.headerTitle = HeaderTitle;
			_listView.showAddRemoveFooter = ShowAddRemoveFooter;
			_listView.allowAdd = AllowAdd;
			_listView.allowRemove = AllowRemove;
			_listView.reorderMode = ReorderMode;
			_listView.showBoundCollectionSize = ShowBoundCollectionSize;
			_listView.bindingSourceSelectionMode = BindingSourceSelectionMode;
			MakeCell = ItemTemplate.Instantiate;
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
	}
	
	public sealed class GridRow : VisualElement
	{
		public static readonly string GridRowClass = "grid-row";
			
		public GridRow(VGridListView gridListView)
		{
			_gridView = gridListView;
				
			RegisterCallbackOnce<AttachToPanelEvent>(OnAttachedToPanel);
		}

		private void OnAttachedToPanel(AttachToPanelEvent evt)
		{
			ClearClassList();
			AddToClassList(GridRowClass);
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
					var visualElement = _gridView.MakeCell();
					_gridElements.Add(visualElement);
				}
			}

			Clear();
				
			for (var i = 0; i < width; i++)
			{
				var visualElement = _gridElements[i];
				Add(visualElement);
					
				var data = rowData.Grid.GetValue(rowData.Row, i);
				_gridView.BindCell(visualElement, data);
			}
		}
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
}