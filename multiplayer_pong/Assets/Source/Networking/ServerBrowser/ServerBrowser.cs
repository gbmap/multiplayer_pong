using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace MultiplayerPong.Networking
{
    public class ServerBrowser
    {
        NetworkManager _networkManager;
        IServerBrowser _serverBrowserBackend;

        public ServerBrowser(
            NetworkManager networkManager, 
            IServerBrowser browser
        ) {
            _networkManager = networkManager;
            _serverBrowserBackend = browser;
        }

        public void GetRooms(System.Action<List<Room>> callback)
        {
            _serverBrowserBackend.GetRooms(callback);
        }

        public void CreateRoom()
        {
            _serverBrowserBackend.CreateRoom((room) => {
                _networkManager.CurrentRoom = room;
                _networkManager.IsServer = true;
                SceneManager.LoadScene("Lobby");
            });
        }

        public void JoinRoom(Room room)
        {
            _serverBrowserBackend.JoinRoom(room, (success) => {
                if (success)
                {
                    _networkManager.CurrentRoom = room;
                    SceneManager.LoadScene("Lobby");
                }
            });
        }

        public void LeaveRoom()
        {
            if (_networkManager.CurrentRoom != null)
            {
                _networkManager.CurrentRoom = null;
                _serverBrowserBackend.LeaveRoom(() => {
                    SceneManager.LoadScene("Servers");
                });
            }
        }
    }
}