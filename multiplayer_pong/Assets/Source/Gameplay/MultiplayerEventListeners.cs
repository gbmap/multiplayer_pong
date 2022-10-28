using System.Collections;
using System.Collections.Generic;
using MultiplayerPong;
using Photon.Bolt;
using UnityEngine;
using Zenject;

namespace MultiplayerPong
{
    public class MultiplayerEventListeners : Photon.Bolt.GlobalEventListener
    {
        [Inject] SignalBus _signalBus;

        void Start()
        {
            if (!BoltNetwork.IsRunning)
                Destroy(this);
        }

        // Handled by both.
        public override void EntityDetached(BoltEntity entity)
        {
            Debug.Log("Entity detached.");
            if (entity.CompareTag("Pad"))
                _signalBus.Fire<SignalOnPlayerDisconnected>();
        }

        // Handled by server
        public override void Disconnected(BoltConnection connection)
        {
            _signalBus.Fire<SignalOnPlayerDisconnected>();
        }

    }
}