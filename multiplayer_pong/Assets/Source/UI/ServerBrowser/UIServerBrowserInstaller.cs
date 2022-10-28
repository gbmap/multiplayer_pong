using UnityEngine;
using Zenject;

namespace MultiplayerPong.UI
{
    public class UIServerBrowserInstaller : MonoInstaller
    {
        public UIServerListItem ServerListItemPrefab;

        public override void InstallBindings()
        {
            Container.Bind<UIServerListItem>().FromInstance(ServerListItemPrefab);
        }
    }
}