using Photon.Bolt;
using UnityEngine;

public class UIVictoryScreen : MonoBehaviour
{
    [SerializeField] protected GameObject VictoryWindow;
    [SerializeField] protected TMPro.TextMeshProUGUI TextVictoryMessage;

    private bool hasSentRequest = false;

    void Awake()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltNetwork.AddGlobalEventCallback<Photon.Bolt.OnPlayerWon>(
                Callback_OnPlayerWon
            );
        }
    }

    void OnDestroy()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltNetwork.RemoveGlobalEventCallback<Photon.Bolt.OnPlayerWon>(
                Callback_OnPlayerWon
            );
        }
    }

    private void Callback_OnPlayerWon(Photon.Bolt.OnPlayerWon msg)
    {
        Debug.Log("Player won.");
        UpdateScreen(msg.PlayerIndex);
        ShowScreen();
    }

    private void ShowScreen()
    {
        VictoryWindow.gameObject.SetActive(true);
    }

    private void UpdateScreen(bool playerIndex)
    {
        TextVictoryMessage.text = GetPlayerWonMessage(playerIndex);
    }

    private string GetPlayerWonMessage(bool playerIndex)
    {
        return !playerIndex ? "Player 1 won!" : "Player 2 won!";
    }

    public void Callback_BtnReplay()
    {
        if (!hasSentRequest)
        {
            Photon.Bolt.RequestReplay.Create().Send();
            hasSentRequest = true;
        }
    }

    public void Callback_BtnBackToLobby()
    {
        Photon.Bolt.RequestBackToLobby.Create().Send();
        hasSentRequest = true;
    }
}
