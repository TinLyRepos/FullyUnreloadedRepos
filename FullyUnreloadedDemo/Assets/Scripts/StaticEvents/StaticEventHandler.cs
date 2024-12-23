using System;
using UnityEngine;

public class StaticEventHandler : MonoBehaviour
{
    // Room changed event
    public static event Action<RoomChangedEventArgs> OnRoomChanged;
    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }

    // Room enemies defeated event
    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;
    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs() { room = room });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedArgs : EventArgs
{
    public Room room;
}