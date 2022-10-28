using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace MultiplayerPong
{
    public class GameplayNetworkController : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GameplayController _gameplayController;
        private readonly NetworkManager _networkManager;

        public System.Action OnReplayRequest;

        public GameplayNetworkController(
            SignalBus signalBus, 
            GameplayController gameplayController,
            NetworkManager networkManager
        ) {
            _signalBus = signalBus; 
            _gameplayController = gameplayController;
            _networkManager = networkManager;
        }

        public void Initialize()
        {
            if (_networkManager.IsServer)
            {
                _signalBus.Subscribe<SignalOnPlayerScored>(Callback_OnPlayerScored);
                _signalBus.Subscribe<SignalOnPlayerWin>(Callback_OnPlayerWon);
            }
            _signalBus.Subscribe<SignalOnPlayerDisconnected>(Callback_OnPlayerDisconnected);
        }

        private void Callback_OnPlayerScored(SignalOnPlayerScored msg)
        {
            Photon.Bolt.OnPlayerScored evnt = Photon.Bolt.OnPlayerScored.Create();
            evnt.PlayerIndex = msg.Score == EScore.Player2Scored;
            evnt.Score = msg.Score == EScore.Player1Scored 
                       ? _gameplayController.Score.x 
                       : _gameplayController.Score.y;
            evnt.Send();
        }

        private void Callback_OnPlayerWon(SignalOnPlayerWin msg)
        {
            Photon.Bolt.OnPlayerWon evnt = Photon.Bolt.OnPlayerWon.Create();
            evnt.PlayerIndex = msg.Score == EScore.Player2Scored;
            evnt.Send();
        }

        private void Callback_OnPlayerDisconnected()
        {
            if (_networkManager.IsServer)
                BoltNetwork.LoadScene("Lobby");
            else    
            {
                Debug.Log("Attempting to go back to servers scene.");
                SceneManager.LoadScene("Servers");
            }
        }


    }
}