using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private string lobbyPlayerPrefabName;
    public Transform myPlayerHolder;
    public Transform otherPlayerHolder;

    private void Start()
    {
        //Spawn All Lobby Players..
        SpawnLobbyPlayer();
    }

    private void SpawnLobbyPlayer()
    {
        PhotonNetwork.Instantiate(lobbyPlayerPrefabName, new Vector3(), Quaternion.identity);
    }
}
