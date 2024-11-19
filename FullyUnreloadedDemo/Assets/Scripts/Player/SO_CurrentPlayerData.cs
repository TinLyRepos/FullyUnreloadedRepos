using UnityEngine;

[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player")]
public class SO_CurrentPlayerData : ScriptableObject
{
    public SO_PlayerData playerData = default;
    public string playerName = string.Empty;
}