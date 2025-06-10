using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    [RequireComponent(typeof(UIDocument))]
    public class GridListView : MonoBehaviour
    {
        private UIDocument _document;
        private VGridListView _gridListView;
        
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            _gridListView = _document.rootVisualElement.Q<VGridListView>();
            
            _gridListView.MakeCell = _visualTreeAsset.CloneTree;
            _gridListView.BindCell = BindCell;
            
            var ints = new int[25, 5];
            for (var y = 0; y < 25; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    ints[y, x] = y;
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
