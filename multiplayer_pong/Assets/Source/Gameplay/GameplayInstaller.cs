using System;
using Photon.Bolt;
using UnityEngine;
using Zenject;

namespace MultiplayerPong
{
    public enum EScore
    {
        None,
        Player1Scored,
        Player2Scored
    }

    public class SignalOnPlayerScored
    {
        public EScore Score { get; private set; }
        public SignalOnPlayerScored(EScore score)
        {
            Score = score;
        }
    }

    public class SignalOnPlayerWin : SignalOnPlayerScored
    {
        public SignalOnPlayerWin(EScore score)
            : base(score) { }
    }

    public class SignalOnPlayerDisconnected {}

    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayController.Settings gameplaySettings;

        public override void InstallBindings()
        {
            InstallSignals();
            InstallGameplayController();
            InstallGameplayNetworkController();
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<SignalOnPlayerScored>().OptionalSubscriber();
            Container.DeclareSignal<SignalOnPlayerWin>().OptionalSubscriber();
            Container.DeclareSignal<SignalOnPlayerDisconnected>().OptionalSubscriber();
        }

        private void InstallGameplayController()
        {
            if (BoltNetwork.IsRunning)
                Container.Bind<IPadSpawner>().To<BoltPadSpawner>().AsSingle();
            else
                Container.Bind<IPadSpawner>().To<UnityPadSpawner>().AsSingle();

            Container.BindInterfacesAndSelfTo<GameplayController>().AsSingle();
            Container.Bind<GameplayController.Settings>().FromInstance(gameplaySettings);
        }

        private void InstallGameplayNetworkController()
        {
            if (BoltNetwork.IsRunning)
                Container.BindInterfacesAndSelfTo<GameplayNetworkController>().AsSingle();
        }
    }
}