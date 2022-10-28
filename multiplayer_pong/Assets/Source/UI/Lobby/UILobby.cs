using System.Linq;
using MultiplayerPong.Networking;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace MultiplayerPong.UI
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI TextRoomName;
        [SerializeField] private Button StartGameButton;

        [Inject] public NetworkManager NetworkManager;
        [Inject] public ServerBrowser ServerBrowser;

        private void Start()
        {
            TextRoomName.text = NetworkManager.CurrentRoom.Name;
            StartGameButton.gameObject.SetActive(NetworkManager.IsServer);
        }

        void Update()
        {
            StartGameButton.gameObject.SetActive(NetworkManager.IsServer && BoltNetwork.Connections.Count() > 0);
        }

        public void LeaveRoom()
        {
            Debug.Log("Leaving room");
            ServerBrowser.LeaveRoom();
        }

        public void StartGame()
        {
            BoltNetwork.LoadScene("Gameplay");
        }

    }
}