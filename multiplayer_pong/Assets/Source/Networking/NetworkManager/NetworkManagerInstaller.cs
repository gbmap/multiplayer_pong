using UnityEngine;
using Zenject;

public class NetworkManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<NetworkManager>().AsSingle();
    }
}