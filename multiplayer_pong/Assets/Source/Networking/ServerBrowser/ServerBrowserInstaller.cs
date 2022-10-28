using UnityEngine;
using Zenject;

namespace MultiplayerPong.Networking.Browser
{
    public class ServerBrowserInstaller : MonoInstaller
    {
        public enum EServerBrowserType
        {
            Photon,
            Dummy
        }
        [SerializeField] private EServerBrowserType _serverBrowserType;

        public override void InstallBindings()
        {
            InstallServerBackend();
        }

        void InstallServerBackend()
        {
            if (_serverBrowserType == EServerBrowserType.Photon)
            {
                var photonBrowser = Container.InstantiateComponentOnNewGameObject<PhotonServerBrowser>();
                Container.Bind<IServerBrowser>().FromInstance(photonBrowser).AsSingle();
            }
            else if (_serverBrowserType == EServerBrowserType.Dummy)
                Container.Bind<IServerBrowser>().To<DummyServerBrowser>().AsSingle();

            Container.Bind<ServerBrowser>().AsSingle();
        }
    }
}