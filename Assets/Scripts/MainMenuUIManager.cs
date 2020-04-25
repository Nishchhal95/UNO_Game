using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI welcomeText;
    private const string WELCOME_TEXT_FORMAT = "Welcome {0}";
    private const string PLAYER_NAME_KEY = "PlayerName";
    [SerializeField] private Button changePlayerNameButton;

    [Header("NoNameUI")]
    [SerializeField] private GameObject noNameUIGO;
    [SerializeField] private GameObject noNameTweenableBaseGO;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button playerNameSubmitButton;

    [Header("PhotonUI")]
    [SerializeField] private GameObject photonUIGO;
    [SerializeField] private GameObject photonTweenableBaseGO;
    [SerializeField] private Button gameCreateButton;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private Button gameJoinButton;

    public delegate void OnMyPlayerNameChanged(string newName);
    public static OnMyPlayerNameChanged onMyPlayerNameChanged;

    private void Start()
    {
        //If No name saved then enable NoName Dialog..
        GameLoad();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        playerNameSubmitButton.onClick.AddListener(OnSaveNameClick);
        gameCreateButton.onClick.AddListener(OnGameCreateClick);
        gameJoinButton.onClick.AddListener(OnGameJoinClick);
        changePlayerNameButton.onClick.AddListener(EnableNoNameDialog);

        onMyPlayerNameChanged += PlayerNameChanged;
        NetworkManager.onMyPlayerJoinedRoom += MyPlayerJoinedRoom;
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        playerNameSubmitButton.onClick.RemoveListener(OnSaveNameClick);
        gameCreateButton.onClick.RemoveListener(OnGameCreateClick);
        gameJoinButton.onClick.RemoveListener(OnGameJoinClick);
        changePlayerNameButton.onClick.RemoveListener(EnableNoNameDialog);

        onMyPlayerNameChanged -= PlayerNameChanged;
        NetworkManager.onMyPlayerJoinedRoom -= MyPlayerJoinedRoom;
    }

    #region Main UI
    private void OnPlayButtonClick()
    {
        //Open Photon UI
        EnablePhotonUI();
    }

    private void PlayerNameChanged(string newPlayerName)
    {
        welcomeText.SetText(string.Format(WELCOME_TEXT_FORMAT, newPlayerName));
    }

    private void MyPlayerJoinedRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion


    #region NO NAME
    private void GameLoad()
    {
        if(!PlayerPrefs.HasKey(PLAYER_NAME_KEY) || string.IsNullOrEmpty(PlayerPrefs.GetString(PLAYER_NAME_KEY, "")))
        {
            //Enable No Name Dialog
            EnableNoNameDialog();
            return;
        }

        string localPlayerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "");

        SetPhotonPlayerName(localPlayerName);
    }

    private void EnableNoNameDialog()
    {
        noNameTweenableBaseGO.transform.localScale = new Vector3(0, 0, 0);
        noNameUIGO.SetActive(true);
        LeanTween.scale(noNameTweenableBaseGO, new Vector3(1, 1, 1), .25f).setEaseInCubic();
    }

    private void CloseNoNameDialog()
    {
        LeanTween.scale(noNameTweenableBaseGO, new Vector3(0, 0, 0), .25f).setEaseInCubic().setOnComplete(() => { noNameUIGO.SetActive(false); });
    }

    private void OnSaveNameClick()
    {
        string localPlayerName = playerNameInputField.text;
        localPlayerName = localPlayerName.Trim();
        localPlayerName = localPlayerName.Replace(" ", "");

        if (string.IsNullOrEmpty(localPlayerName))
        {
            //TODO : Show a message as name cannot be blank

            return;
        }

        SetPhotonPlayerName(localPlayerName);
        CloseNoNameDialog();
    }

    private void SetPhotonPlayerName(string playerLocalName)
    {
        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerLocalName);
        onMyPlayerNameChanged?.Invoke(playerLocalName);
        Photon.Pun.PhotonNetwork.LocalPlayer.NickName = playerLocalName;
    }

    #endregion


    #region Photon UI

    private void EnablePhotonUI()
    {
        photonTweenableBaseGO.transform.localScale = new Vector3(0, 0, 0);
        photonUIGO.SetActive(true);
        LeanTween.scale(photonTweenableBaseGO, new Vector3(1, 1, 1), .25f).setEaseInCubic();
    }

    private void ClosePhotonUI()
    {
        LeanTween.scale(photonTweenableBaseGO, new Vector3(0, 0, 0), .25f).setEaseInCubic().setOnComplete(() => { photonUIGO.SetActive(false); });
    }

    private void OnGameCreateClick()
    {
        string roomName = GiveRandomRoomName();

        NetworkManager.Instance.CreateRoom(roomName);

        ClosePhotonUI();
    }

    private void OnGameJoinClick()
    {
        string roomName = roomCodeInputField.text;
        roomName = roomName.Trim();

        if(string.IsNullOrEmpty(roomName))
        {
            //TODO : Show a message as room name cannot be empty

            return;
        }

        NetworkManager.Instance.JoinRoom(roomName);
        ClosePhotonUI();
    }

    #endregion

    //Other Functions
    private string GiveRandomRoomName()
    {
        string randomRoomName = "";
        do
        {
            int randomNumber = Random.Range(1000, 99999);
            randomRoomName = randomNumber.ToString();
        } while (NetworkManager.Instance.RoomExists(randomRoomName));

        return randomRoomName;
    }
}
