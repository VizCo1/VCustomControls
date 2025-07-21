using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents.Runtime
{
	[UxmlElement]
	public partial class VGridListView : VisualElement
	{
		public static readonly string VGridListViewClass = "grid-list-view";
		
		private readonly ListView _listView;
		
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
		
		public Action<VisualElement, int> BindCell { get; set; }
		
		public Func<VisualElement> MakeCell { get; private set; }
		
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
#if UNITY_EDITOR
			if (panel.contextType == ContextType.Editor || !Application.isPlaying)
				return;
#endif
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

		public void BindToGrid(int[,] grid)
		{
			var height = grid.GetLength(0);

			var rowData = new List<VGridRowData>();
			for (var i = 0; i < height; i++)
			{
				var data = new VGridRowData(i, grid);
				rowData.Add(data);
			}

			_listView.itemsSource = rowData;
		}
		
		private VisualElement MakeItem()
		{
			return new VGridRow(this);
		}

		private void BindItem(VisualElement visualElement, int index)
		{
			var rowData = (VGridRowData)_listView.itemsSource[index];
			
			var gridRow = (VGridRow)visualElement;
			
			var isLastRow = index == _listView.itemsSource.Count - 1;
			
			gridRow.EnableInClassList(VGridRow.VLastGridRowClass, isLastRow);
			
			gridRow.BindToGridRowData(rowData);
		}
	}
}