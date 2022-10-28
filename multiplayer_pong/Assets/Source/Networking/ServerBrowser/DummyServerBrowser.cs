using System;
using System.Collections.Generic;

public class DummyServerBrowser : IServerBrowser
{
    public void JoinRoom(Room room, Action<bool> callback)
    {
        callback(true);
    }

    public void CreateRoom(Action<Room> callback)
    {
        callback(new Room(Guid.NewGuid()));
    }

    public void GetRooms(Action<List<Room>> callback)
    {
        callback(new List<Room>() {
            new Room(Guid.NewGuid()),
            new Room(Guid.NewGuid()),
            new Room(Guid.NewGuid()),
            new Room(Guid.NewGuid()),
            new Room(Guid.NewGuid()),
        });
    }

    public void LeaveRoom(Action callback)
    {
        callback();
    }
}