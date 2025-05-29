using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        //public PlayerInput playerInput;
        public string controlScheme;
        public InputDevice device;
        public Color colour;
        public int score;

        public PlayerInfo(string cs,InputDevice id)
        {
            controlScheme = cs;
            device = id;
            score = 0;
            colour = new Color(Random.value, Random.value, Random.value);
        }
        public PlayerInfo(PlayerInput input)
        {
            controlScheme = "Gamepad";
            device = input.devices[0];
            score = 0;
            colour = new Color(Random.value, Random.value, Random.value);
        }
    }


    public static GameManager instance;

    [Min(0)]
    public int scoreToWin = 5;

    //Game Data
    [HideInInspector]
    public int playerCount = 0;
    public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    [HideInInspector]
    public int roundNumber = 0;

    public bool Testing = false;

    //singleton 
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += onStartLoad;

        if (Testing)
        {
            AddPlayer("Keyboard1", Keyboard.current);
            AddPlayer("keyboard2", Keyboard.current);
        }
    }

    void onStartLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Start")
        {
            InitGame();
        }
;   
    }


    /// <summary>
    /// Reset all game data. Use when done with a game.
    /// </summary>
    public void InitGame()
    {
        playerCount = 0;
        roundNumber = 0;
        playerInfoList = new List<PlayerInfo>();
    }

    public int GetScore(int playerIndex)
    {
        return playerInfoList[playerIndex].score;
    }

    public void SetScore(int playerIndex,int newScore)
    {
        playerInfoList[playerIndex].score = newScore;
    }

    public void AddPlayer(PlayerInput newPlayer)
    {
        playerInfoList.Add(new PlayerInfo(newPlayer));
        playerCount++;
    }
    public void AddPlayer(string controlSceme,InputDevice inputDevice)
    {
        playerInfoList.Add(new PlayerInfo(controlSceme, inputDevice));
        playerCount++;
    }
    
}
