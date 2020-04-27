using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    private PhotonView myPhotonView;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] LobbyManager lobbyManager;

    [SerializeField] private string playerPrefabName;
    [SerializeField] private GameObject myGamePlayer;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        lobbyManager = FindObjectOfType<LobbyManager>();
        InitializeMyPlayer();
    }

    //private void OnEnable()
    //{
    //    LobbyCanvasManager.onGameStartClick += StartGame;
    //}

    //private void OnDisable()
    //{
    //    LobbyCanvasManager.onGameStartClick -= StartGame;
    //}

    private void InitializeMyPlayer()
    {
        if (myPhotonView.IsMine)
        {
            transform.SetParent(lobbyManager.myPlayerHolder);
        }

        else
        {
            transform.SetParent(lobbyManager.otherPlayerHolder);
        }

        transform.localPosition = new Vector3();
        playerNameText.SetText(myPhotonView.Owner.NickName);
    }

    //private void StartGame()
    //{
    //    if (myPhotonView.Owner.IsMasterClient)
    //    {
    //        myPhotonView.RPC("SpawnGamePlayer_RPC", RpcTarget.AllBufferedViaServer, new object[] { });
    //    }
    //}

    //[PunRPC]
    //public void SpawnGamePlayer_RPC()
    //{
    //    myGamePlayer = PhotonNetwork.Instantiate(playerPrefabName, new Vector3(), Quaternion.identity);

    //    LobbyCanvasManager.hideLobbyUI?.Invoke();
    //}
}
