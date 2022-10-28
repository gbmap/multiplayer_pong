using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerPong.UI
{
    public class UIServerListItem : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI TextRoomName;

        Room _room;
        public Room Room
        {
            set
            {
                _room = value;
                TextRoomName.text = _room.Name;
            }
            get => _room;
        }
    }
}