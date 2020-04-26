using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance = null;

    //Photon Settings
    [SerializeField] private bool isConnected = false;

    public delegate void OnPhotonConnectedToMaster();
    public static OnPhotonConnectedToMaster onPhotonConnectedToMaster;

    public delegate void OnMyPlayerJoinedRoom();
    public static OnMyPlayerJoinedRoom onMyPlayerJoinedRoom;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(this);
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        onPhotonConnectedToMaster += OnPhotonNetworkGotConnected;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        onPhotonConnectedToMaster -= OnPhotonNetworkGotConnected;
    }

    private void OnPhotonNetworkGotConnected()
    {
        isConnected = true;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Public Methods

    public bool IsConnected()
    {
        return isConnected;
    }

    public int GetPlayerCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public bool RoomExists(string roomName)
    {
        if (!IsConnected())
        {
            return true;
        }

        return PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "C0=" + roomName);
    }

    public List<Player> GetCurrentRoomPlayers()
    {
        return PhotonNetwork.CurrentRoom.Players.Values.ToList();
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void CreateRoom(string roomName, int playerCount)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)playerCount
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        onPhotonConnectedToMaster?.Invoke();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: " + PhotonNetwork.CurrentRoom.Name);
        onMyPlayerJoinedRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom: " + PhotonNetwork.CurrentRoom.Name + ", " + newPlayer.NickName);
    }

    #endregion
}
