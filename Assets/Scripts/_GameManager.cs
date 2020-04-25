using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class _GameManager : MonoBehaviour
{
    [SerializeField] private string playerPrefabName;
    [SerializeField] private GameObject myPlayerGO;

    private void Start()
    {
        //TODO : Show players waiting for Master Client to start the game...
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            myPlayerGO = PhotonNetwork.Instantiate(playerPrefabName, new Vector3(0, PhotonNetwork.CurrentRoom.PlayerCount * 2, 0), Quaternion.identity);
            myPlayerGO.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(PhotonNetwork.LocalPlayer.NickName);
        }
    }
}
