using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using UdpKit.Platform;
using UnityEngine;

public class PhotonServerBrowser : Photon.Bolt.GlobalEventListener, IServerBrowser
{
    private class RoomCache
    {
        public Room Room { get; private set; }
        public UdpSession Session { get; private set; }
        
        public RoomCache(Room room, UdpSession session)
        {
            Room = room;
            Session = session;
        }
    }

    List<RoomCache> _roomsCache = new List<RoomCache>();

    private bool isServer;
    private bool hasStarted;

    void Awake()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.Log("Session list updated...");
        _roomsCache = sessionList.Select(
            s => new RoomCache(new Room(s.Key), s.Value)
        ).ToList();
    }

    public override void BoltStartBegin()
    {
        base.BoltStartBegin();
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();
        Debug.Log("Bolt started");
        hasStarted = true;
    }

    public void JoinRoom(Room room, Action<bool> callback)
    {
        StartCoroutine(JoinRoomCoroutine(room, callback));
    }

    private IEnumerator JoinRoomCoroutine(Room room, Action<bool> callback)
    {
        yield return StartClient();

        var session = _roomsCache.Find(r => r.Room.Guid == room.Guid).Session;
        BoltMatchmaking.JoinSession(session);
        yield return new WaitUntil(() => BoltNetwork.IsConnected && BoltMatchmaking.CurrentSession != null);

        callback(true);
    }

    public void CreateRoom(Action<Room> callback)
    {
        StartCoroutine(CreateRoomCoroutine(callback));
    }

    private IEnumerator CreateRoomCoroutine(Action<Room> callback)
    {
        // Make sure this is not a client version of Bolt running
        yield return StartServer();
        Guid id = GetRoomId();

        BoltMatchmaking.CreateSession(
            sessionID: id.ToString()
        );
        yield return new WaitUntil(() => BoltNetwork.IsServer && BoltMatchmaking.CurrentSession != null);

        callback(new Room(BoltMatchmaking.CurrentSession.Id));
    }
    
    private IEnumerator StartClient()
    {
        if (BoltNetwork.IsServer)
        {
            BoltLauncher.Shutdown();
            yield return new WaitUntil(() => !BoltNetwork.IsRunning);
            yield return new WaitForSeconds(1f);
        }

        if (!BoltNetwork.IsRunning)
        {
            BoltLauncher.StartClient();
            yield return new WaitUntil(() => BoltNetwork.IsRunning);
        }

        yield return new WaitUntil(() => hasStarted);
    }


    private IEnumerator StartServer()
    {
        if (BoltNetwork.IsClient)
        {
            BoltLauncher.Shutdown();
            yield return new WaitUntil(() => !BoltNetwork.IsRunning);
            yield return new WaitForSeconds(1f);
        }

        if (!BoltNetwork.IsRunning)
        {
            BoltLauncher.StartServer();
            yield return new WaitUntil(() => BoltNetwork.IsServer && BoltNetwork.IsRunning);
        }
        
        yield return new WaitUntil(() => hasStarted);
    }

    Guid GetRoomId()
    {
        return Guid.NewGuid();
    }

    public void GetRooms(Action<List<Room>> callback)
    {
        if (!BoltNetwork.IsRunning)
            BoltLauncher.StartClient();

        callback(_roomsCache.Select(r => r.Room).ToList());
    }

    private IEnumerator GetRoomsCoroutine()
    {
        if (!BoltNetwork.IsRunning)
        {
            BoltLauncher.StartClient();
            yield return new WaitUntil(() => BoltNetwork.IsRunning);
        }

    }

    public void LeaveRoom(Action callback)
    {
        StartCoroutine(LeaveRoomCoroutine(callback));
    }

    private IEnumerator LeaveRoomCoroutine(Action callback)
    {
        BoltLauncher.Shutdown();
        yield return new WaitUntil(() => !BoltNetwork.IsRunning);
        callback();
    }

}