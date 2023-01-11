using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//using System;

public class PlayerHighscores
{
    private FileStream fileStream;
    private BinaryFormatter formatter = new BinaryFormatter();
    private ListPlayerHighscore playerHighscores;
    private string strSaveFilePath = Application.persistentDataPath + "/playerHighscores.data";

    public enum SortOrder { NAME, HIGHSCORE }
    public SortOrder sortOrder;
    public PlayerHighscores()
    {
        sortOrder  = SortOrder.NAME;
        LoadPlayerHighscores();
    }

    ~PlayerHighscores()
    {
        SavePlayerHighscores();
    }
    
    // Load the player highscores from file
    private void LoadPlayerHighscores()
    {
        if (File.Exists(strSaveFilePath))
        {
            fileStream = new FileStream(strSaveFilePath, FileMode.Open);
            playerHighscores = (ListPlayerHighscore)formatter.Deserialize(fileStream);
            fileStream.Close();
        }
        else
        {
            playerHighscores = new ListPlayerHighscore();
            playerHighscores.listPlayerHighscores = new List<PlayerHighscore>();
        }
    }

    // Saves the player highscores to file
    public void SavePlayerHighscores()
    {
        fileStream = new FileStream(strSaveFilePath, FileMode.OpenOrCreate, FileAccess.Write);
        formatter.Serialize(fileStream, playerHighscores);
        fileStream.Close();
    }

    // Add a player to the list
    public bool AddPlayer(string strPlayerName)
    {
        if(PlayerExists(strPlayerName))
        {
            Debug.Log("Player " + strPlayerName + " already exists!");
            return false;
        }
        PlayerHighscore playerData = new PlayerHighscore();
        playerData.playerName = strPlayerName;
        playerData.highScore = 0;
        playerHighscores.listPlayerHighscores.Add(playerData);
        return true;
    }

    public bool PlayerExists(string strPlayerName)
    {
        PlayerHighscore playerHighscore;
        playerHighscore = playerHighscores.listPlayerHighscores.Find(player => player.playerName == strPlayerName);
        if (playerHighscore != null)
            return true;

        return false;
    }

    // Get a player's highscore
    public int getPlayerHighscore(string strPlayerName)
    {
        PlayerHighscore playerHighscore;
        playerHighscore = playerHighscores.listPlayerHighscores.Find(player => player.playerName == strPlayerName);

        if (playerHighscore != null)
            return playerHighscore.highScore;
        else
        {
            Debug.Log("Player " + strPlayerName + " was not found!");
            return -1;
        }
    }

    // Set a player's highscore
    public void setPlayerHighscore(string strPlayerName, int score)
    {
        if (playerHighscores.listPlayerHighscores.Find(player => player.playerName == strPlayerName) != null)
        {
            PlayerHighscore p = playerHighscores.listPlayerHighscores.Find(player => player.playerName == strPlayerName);
            int playerHighScore = p.highScore;
            if (score > playerHighScore)
                p.highScore = score;
        }
        else
            Debug.Log("Player " + strPlayerName + " was not found!");
    }

    // Get a list of all player highscores
    public List<PlayerHighscore> getPlayerHighScores(SortOrder order = SortOrder.NAME)
    {
        List<PlayerHighscore> listPlayerHighScores = new List<PlayerHighscore>(10);
        
        foreach(PlayerHighscore highScore in playerHighscores.listPlayerHighscores)
        {
            listPlayerHighScores.Add(highScore);
        }
        
        if(order == SortOrder.NAME)
            listPlayerHighScores.Sort((PlayerHighscore p1, PlayerHighscore p2) => p1.playerName.CompareTo(p2.playerName));
        else
            listPlayerHighScores.Sort((PlayerHighscore p1, PlayerHighscore p2) => p2.highScore.CompareTo(p1.highScore));

        return listPlayerHighScores;
    }

    // Get the highest score 
    public PlayerHighscore getHighscore()
    {
        List<PlayerHighscore> playerHighscores = getPlayerHighScores(SortOrder.HIGHSCORE);
        if(playerHighscores.Count > 0)
            return playerHighscores[0];
        else
        {
            PlayerHighscore p = new PlayerHighscore();
            p.playerName = "NO PLAYER FOUND";
            p.highScore = -1;
            return p;
        }
    }

    public string getHighscoreText()
    {
        return "Highscore: " + getHighscore().playerName + "  " + getHighscore().highScore;
    }
}

[System.Serializable]
public class ListPlayerHighscore
{
    public List<PlayerHighscore> listPlayerHighscores;
}

[System.Serializable]
public class PlayerHighscore// : IComparable<PlayerHighscore>
{
    public string playerName = "";
    public int highScore = -1;

   /* public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as PlayerHighscore;

        return string.Compare(a.playerName, b.playerName);
    }*/
   /*
    public int CompareTo(PlayerHighscore playerHighscore)
    {
        var a = this;
        return string.Compare(a.playerName, playerHighscore.playerName);
        return 0;
    }

    public int Compare(PlayerHighscore playerHighscore, PlayerHighscore p2)
    {
        if (this.highScore < playerHighscore.highScore)
            return 1;
        else if (this.highScore > playerHighscore.highScore)
            return -1;
        return 0;
    }*/
}
