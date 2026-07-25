using UnityEngine;

[CreateAssetMenu(fileName = "LobbyEntryData", menuName = "Scriptable Objects/LobbyEntryData")]
public class LobbyEntryData : ScriptableObject
{
    [field: SerializeField]
    public string[] ViewNames { get; private set; }
}
