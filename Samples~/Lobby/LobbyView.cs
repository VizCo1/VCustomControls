using UnityEngine;
using UnityEngine.UIElements;
using UserInterfaceGenerator;

namespace VCustomComponents.Runtime
{
    public class LobbyView : VBaseView<LobbyElements>
    {
        [SerializeField]
        private LobbyEntryData _lobbyEntryData;
        
        [SerializeField]
        private int _columns = 4;
        
        protected override void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
        {
            Elements.LobbyList.BindCell = BindCell;
            
            var rows = Mathf.CeilToInt(_lobbyEntryData.ViewNames.Length / (float)_columns);
            
            var cellIndex = 0;
            var grid = new int[rows, _columns];
            for (var y = 0; y < grid.GetLength(0); y++)
            {
                for (var x = 0; x < grid.GetLength(1); x++)
                {
                    if (cellIndex >= _lobbyEntryData.ViewNames.Length)
                    {
                        grid[y, x] = -1;
                        continue;
                    }
                    
                    grid[y, x] = cellIndex++;
                }
            }
            
            Elements.LobbyList.BindToGrid(grid);
        }

        private void BindCell(VisualElement visualElement, int index)
        {
            if (index == -1)
                return;
            
            var button = visualElement.Q<Button>();
            
            button.UnregisterCallback<ClickEvent, int>(OnCellClicked);
            button.RegisterCallback<ClickEvent, int>(OnCellClicked, index);
            
            button.text = _lobbyEntryData.ViewNames[index];
        }

        private void OnCellClicked(ClickEvent evt, int index)
        {
            if (index == -1)
                return;
            
            UIManager.Instance.PushDocument(index);
        }
    }
}
