using UnityEngine;
using UnityEngine.UIElements;

namespace VCustomComponents
{
    public abstract class BaseView : MonoBehaviour
    {
        protected UIDocument _document;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }
    }
}
