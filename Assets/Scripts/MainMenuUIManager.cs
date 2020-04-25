using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI welcomeText;
    private const string WELCOME_TEXT_FORMAT = "Welcome {0}";

    [Header("NoNameUI")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button playerNameSubmitButton;

    [Header("PhotonUI")]
    [SerializeField] private Button gameCreateButton;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private Button gameJoinButton;
}
