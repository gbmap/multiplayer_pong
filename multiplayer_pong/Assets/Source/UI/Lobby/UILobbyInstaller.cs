using System.Collections;
using System.Collections.Generic;
using MultiplayerPong.Networking;
using UnityEngine;
using Zenject;

namespace MultiplayerPong.UI
{
    public class UILobbyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Container.BindInterfacesAndSelfTo<UILobby>().AsTransient();
            
        }
    }
}