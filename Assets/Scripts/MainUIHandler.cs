using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainUIHandler : MonoBehaviour
{
    // Some static variables for data transfer between scenes
    public static string strPlayerName = "";
    public static int playerDropdownIndex = -1;
    public static string strBestScoreText = "";
    public static int score = -1;

    private PlayerHighscores playerHighscores;
    private TMP_Dropdown playerDropdown;
    private TMP_Text playerDropdownLabel;
    private TMP_Text messageText;
    private TMP_Text hintText;
    private List<PlayerHighscore> listPlayerHighscores;
    private TextMeshProUGUI playerName;
    private Button buttonStartGame;
    private Button buttonAddPlayer;
    public TMP_Text highscoresText;

    private static bool coroutineDeleteMessageTextStarted = false;

    private void Awake()
    {
        playerHighscores = new PlayerHighscores();
        buttonStartGame = GameObject.Find("ButtonStartGame").GetComponent<Button>();
        buttonAddPlayer = GameObject.Find("ButtonAddPlayer").GetComponent<Button>();
        playerDropdown = GameObject.Find("PlayerDropdown").GetComponent<TMP_Dropdown>();
        playerName = GameObject.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        playerDropdownLabel = GameObject.Find("Label").GetComponent<TMP_Text>();
        messageText = GameObject.Find("MessageText").GetComponent<TMP_Text>();
        hintText = GameObject.Find("HintText").GetComponent<TMP_Text>();
        listPlayerHighscores = playerHighscores.getPlayerHighScores();

        // Fill the player dropdown with values
        foreach (PlayerHighscore playerHighscore in listPlayerHighscores)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(playerHighscore.playerName);
            playerDropdown.options.Add(optionData);  
        }

        // Sort the dropdown alphabetically
        playerDropdown.options.Sort(SortAlphabetically);
        //playerDropdown.options.Sort((TMP_Dropdown.OptionData d1, TMP_Dropdown.OptionData d2) => d1.text.CompareTo(d2.text));

        if (playerDropdownIndex == -1)
            playerDropdownLabel.text = "Select player";
        else
        {
            playerHighscores.setPlayerHighscore(strPlayerName, score);
            playerDropdownLabel.text = strPlayerName;
            playerDropdown.value = playerDropdownIndex;
        }
        strBestScoreText = playerHighscores.getHighscoreText();
        SetHighscoresText();
    }

    public void PlayerDropdownValueChanged()
    {
        buttonStartGame.interactable = true;
        strPlayerName = playerDropdown.options[playerDropdown.value].text;
        playerDropdownLabel.text = strPlayerName;
        string highscore = listPlayerHighscores.Find((PlayerHighscore p) => p.playerName == playerDropdownLabel.text).highScore.ToString();
        playerName.text = "Player: " + strPlayerName + "  Highscore: " + highscore;
        playerDropdownIndex = playerDropdown.value;
    }

    private void SetHighscoresText()
    {
        foreach (PlayerHighscore playerHighscore in playerHighscores.getPlayerHighScores(PlayerHighscores.SortOrder.HIGHSCORE))
        {
            highscoresText.text += playerHighscore.playerName + " " + playerHighscore.highScore.ToString() + "\n";
        }
    }

    public void AddPlayer(TextMeshProUGUI textfield)
    {
        if (textfield.text.Trim().Length > 2)
        {
            if(playerHighscores.PlayerExists(textfield.text))
            {
                SetMessageText("Player " + textfield.text + " already exists!", false, 1.0f);
                return;
            }
            playerHighscores.AddPlayer(textfield.text);
            listPlayerHighscores = playerHighscores.getPlayerHighScores(PlayerHighscores.SortOrder.NAME);
            playerDropdown.options.Add(new TMP_Dropdown.OptionData(textfield.text));
            playerDropdown.options.Sort(SortAlphabetically);
        }
        else
            SetMessageText("Insert a playername with at least 2 characters!", false, 1.0f);
    }

    private void SetMessageText(string strMessage, bool notDelete = false, float deleteAfterSeconds = 1.0f)
    {
        messageText.text = strMessage;
        if(!notDelete && !coroutineDeleteMessageTextStarted)
            StartCoroutine(DeleteMessageText(deleteAfterSeconds));
    }

    private IEnumerator DeleteMessageText(float deleteAfterSeconds)
    {
        coroutineDeleteMessageTextStarted = true;
        yield return new WaitForSeconds(deleteAfterSeconds);
        SetMessageText("");
        coroutineDeleteMessageTextStarted = false;
    }

    private int SortAlphabetically(TMP_Dropdown.OptionData d1, TMP_Dropdown.OptionData d2)
    {
        return d1.text.CompareTo(d2.text);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void ExitGame()
    {
        playerHighscores.SavePlayerHighscores();
        #if UNITY_STANDALONE
                                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }
}