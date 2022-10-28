using System;
using System.Collections.Generic;

public class Room 
{
    // public uint Id { get; private set; }
    public Guid Guid { get; private set; }

    public string Name
    {
        get { return $"{Guid.ToString()}"; }
    }
    
    public Room(Guid guid)
    {
        Guid = guid;
    }

    public Room(string guid)
    {
        Guid = new Guid(guid);
    }
}

public interface IServerBrowser
{
    void JoinRoom(Room room, System.Action<bool> callback);
    void LeaveRoom(System.Action callback);
    void CreateRoom(System.Action<Room> callback);
    void GetRooms(System.Action<List<Room>> callback);
}
