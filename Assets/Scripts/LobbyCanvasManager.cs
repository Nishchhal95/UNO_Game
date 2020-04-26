using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvasManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private GameObject UIGO;

    public delegate void OnGameStartClick();
    public static OnGameStartClick onGameStartClick;

    public delegate void HideLobbyUI();
    public static HideLobbyUI hideLobbyUI;

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(StartGameButtonClick);
        hideLobbyUI += DisableLobbyUI;
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(StartGameButtonClick);
        hideLobbyUI -= DisableLobbyUI;
    }

    private void Start()
    {
        bool isMasterClient = NetworkManager.Instance.IsMasterClient();

        startGameButton.gameObject.SetActive(isMasterClient);
        startGameText.gameObject.SetActive(!isMasterClient);

        if(!isMasterClient)
        {
            string masterClientNickName = NetworkManager.Instance.GetCurrentRoomPlayers().Find(x => x.IsMasterClient).NickName;
            startGameText.SetText("Waiting for " + masterClientNickName + " to Start Game...");
        }
    }

    private void StartGameButtonClick()
    {
        onGameStartClick?.Invoke();
    }

    public void DisableLobbyUI()
    {
        UIGO.SetActive(false);
    }
}
