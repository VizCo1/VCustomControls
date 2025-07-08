using UnityEngine;

namespace VCustomComponents
{
    [CreateAssetMenu(fileName = "VSpriteAnimation", menuName = "VCustomControls/VSpriteAnimation")]
    public class VSpriteAnimation : ScriptableObject
    {
        [field: SerializeField]
        public Sprite[] Sprites { get; private set; }
    }
}
