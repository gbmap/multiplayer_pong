using Photon.Bolt;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    [SerializeField] private bool playerIndex;

    void Awake()
    {
        if (BoltNetwork.IsRunning)
        {
            // BoltNetwork.AddGlobalEventCallback<>(OnPlayerScored);
            BoltNetwork.AddGlobalEventCallback<Photon.Bolt.OnPlayerScored>(
                Callback_OnPlayerScored
            );
        }
    }

    void OnDestroy()
    {
        if (BoltNetwork.IsRunning)
        {
            // BoltNetwork.AddGlobalEventCallback<>(OnPlayerScored);
            BoltNetwork.RemoveGlobalEventCallback<Photon.Bolt.OnPlayerScored>(
                Callback_OnPlayerScored
            );
        }
    }

    private void Callback_OnPlayerScored(Photon.Bolt.OnPlayerScored msg)
    {
        if (msg.PlayerIndex != playerIndex)
            return;
        else
            Text.text = msg.Score.ToString();
    }
}
