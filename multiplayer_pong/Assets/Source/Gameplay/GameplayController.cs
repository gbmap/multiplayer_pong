using UnityEngine;
using Zenject;
using Photon.Bolt;
using MultiplayerPong.Gameplay;
using System;

namespace MultiplayerPong
{
    public class GameplayController : IInitializable, IDisposable
    {
        [System.Serializable]
        public class Settings
        {
            [System.Serializable]
            public class Prefabs
            {
                public GameObject PadPrefab;
                public GameObject BallPrefab;
            }

            public Prefabs Prefab = new Prefabs();
            public Vector3 Pad1StartPosition = new Vector3(-5, 0, 0);
            public Vector3 Pad2StartPosition = new Vector3(5, 0, 0);
            public Vector3 BallStartPosition = new Vector3(0, 0, 0);
            public int MaxScore = 5;
        }

        private readonly NetworkManager NetworkManager;
        private readonly IPadSpawner Spawner;
        private readonly Settings GameplaySettings;
        private readonly SignalBus _signalBus;

        public Vector2Int Score { get; private set; }

        // Used to replay the game.
        private int RequestsRequired { get { return NetworkManager.IsServer ? 2 : 1; } }
        private int replayRequests;


        public GameplayController(
            NetworkManager networkManager, 
            IPadSpawner spawner,
            Settings settings,
            SignalBus signalBus
        ) {
            NetworkManager = networkManager;
            Spawner = spawner;
            GameplaySettings = settings;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            SubscribeToEvents(_signalBus);

            SpawnPlayers(Spawner, !NetworkManager.IsServer);
            SpawnBall(Spawner, !NetworkManager.IsServer);

            Score = Vector2Int.zero;
        }

        public void Dispose()
        {
            if (BoltNetwork.IsRunning)
            {
                BoltNetwork.RemoveGlobalEventCallback<RequestReplay>(Callback_OnReplayRequest);
                BoltNetwork.RemoveGlobalEventCallback<RequestBackToLobby>(Callback_OnBackToLobbyRequest);
            }
        }

        private void SubscribeToEvents(SignalBus signalBus)
        {
            signalBus.Subscribe<SignalOnPlayerScored>(Callback_OnPlayerScored);

            if (BoltNetwork.IsRunning)
            {
                BoltNetwork.AddGlobalEventCallback<RequestReplay>(Callback_OnReplayRequest);
                BoltNetwork.AddGlobalEventCallback<RequestBackToLobby>(Callback_OnBackToLobbyRequest);
            }
        }

        private void Callback_OnPlayerScored(SignalOnPlayerScored score)
        {
            Debug.Log("Player scored: " + score.Score);
            if (score.Score == EScore.Player1Scored)
                Score = new Vector2Int(Score.x+1, Score.y);
            else if (score.Score == EScore.Player2Scored)
                Score = new Vector2Int(Score.x, Score.y+1);

            if (Score.x >= GameplaySettings.MaxScore || Score.y >= GameplaySettings.MaxScore)
            {
                Debug.Log("Player won: " + score.Score);
                _signalBus.Fire<SignalOnPlayerWin>(new SignalOnPlayerWin(score.Score));
            }
        }

        void SpawnPlayers(IPadSpawner spawner, bool playerIndex)
        {
            Vector3 startingPosition = !playerIndex 
                                     ? GameplaySettings.Pad1StartPosition 
                                     : GameplaySettings.Pad2StartPosition;
            GameObject pad = spawner.Spawn(
                GameplaySettings.Prefab.PadPrefab, 
                startingPosition, 
                Quaternion.identity
            );
            pad.gameObject.AddComponent<PadController>();
        }

        public void SpawnBall(IPadSpawner spawner, bool playerIndex)
        {
            // Player 2 client, don't spawn ball.
            if (playerIndex) 
                return;

            GameObject ball = spawner.Spawn(
                GameplaySettings.Prefab.BallPrefab,
                GameplaySettings.BallStartPosition,
                Quaternion.identity
            );
            ball.GetComponent<ISignalBusHolder>().SignalBus = _signalBus;
        }

        private void Callback_OnReplayRequest(RequestReplay msg)
        {
            replayRequests++;
            if (replayRequests >= RequestsRequired)
            {
                BoltNetwork.LoadScene("Gameplay");
            }
        }

        private void Callback_OnBackToLobbyRequest(RequestBackToLobby msg)
        {
            BoltNetwork.LoadScene("Lobby");
        }
    }
}