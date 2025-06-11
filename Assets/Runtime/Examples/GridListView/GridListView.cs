using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class GridListView : MonoBehaviour
    {
        private UIDocument _document;
        private VGridListView _gridListView;
        
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _gridListView = _document.rootVisualElement.Q<VGridListView>();
            
            _gridListView.BindCell = BindCell;

            var columns = 5;
            var rows = 20;
            
            var ints = new int[rows, columns];
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    ints[y, x] = x + y + 10;
                }
            }

            _gridListView.BindToGrid(ints);
        }

        private void BindCell(VisualElement visualElement, object data)
        {
            var button = (Button)visualElement[0];
            
            button.text = data.ToString();
        }
    }
}
