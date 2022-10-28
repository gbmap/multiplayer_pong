using Photon.Bolt;
using UnityEngine;

namespace MultiplayerPong
{
    public interface IPadSpawner
    {
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation);
    }

    public class UnityPadSpawner : IPadSpawner
    {
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GameObject.Instantiate(prefab, position, rotation);
        }
    }

    public class BoltPadSpawner : IPadSpawner
    {
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return BoltNetwork.Instantiate(prefab, position, rotation).gameObject;
        }
    }
}
