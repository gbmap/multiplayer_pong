using System.Collections.Generic;
using UnityEngine;
using MultiplayerPong.Networking;
using Zenject;
using UnityEngine.UI;

namespace MultiplayerPong.UI
{
    public class UIServerList : MonoBehaviour
    {
        [Inject] public ServerBrowser ServerBrowser;
        [Inject, HideInInspector] public UIServerListItem ListItemPrefab;

        [SerializeField] private Transform Container;
        [SerializeField] private UIServerListItem SelectedRoom;

        public void Refresh()
        {
            ClearRooms();
            ServerBrowser.GetRooms((rooms) => {
                PopulateRoom(rooms);
            });
        }

        void ClearRooms()
        {
            for (int i = 0; i < Container.childCount; i++)
            {
                Destroy(Container.GetChild(i).gameObject);
            }
        }

        void PopulateRoom(List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                var listItem = Instantiate(ListItemPrefab, Container);
                ConfigureListItem(listItem, room);
            }
        }

        private void ConfigureListItem(UIServerListItem listItem, Room room)
        {
            listItem.Room = room;
            listItem.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    Debug.Log("Selecting " + room.Name);
                    SelectedRoom = listItem;
                }
            });
        }

        public void JoinSelected()
        {
            Debug.Log("Joining " + SelectedRoom.Room.Name);
            ServerBrowser.JoinRoom(SelectedRoom.Room);
        }

        public void CreateRoom()
        {
            Debug.Log("Creating...");
            ServerBrowser.CreateRoom();
        }
    }
}