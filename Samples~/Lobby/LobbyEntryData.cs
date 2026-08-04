using UnityEngine;

namespace Samples
{
    [CreateAssetMenu(fileName = "LobbyEntryData", menuName = "Scriptable Objects/LobbyEntryData")]
    public class LobbyEntryData : ScriptableObject
    {
        [field: SerializeField]
        public string[] ViewNames { get; private set; }
    }
}
