using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    //settings menu
    public AudioMixer audioMixer;
    public void setVolume(float volume)
    {
        audioMixer.SetFloat("GameVolume", volume);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    //---------------------- PLAYER DEVICES ----------------------------

    int keyboardPlayersCount = 2;
    //play menu
    public void setKeyboardPlayers1(bool toggled)
    {
        if (toggled)
        {
            setKeyboardPlayers(1);
        }
    }

    public void setKeyboardPlayers2(bool toggled)
    {
        if (toggled)
        {
            setKeyboardPlayers(2);
        }
    }
    private void setKeyboardPlayers(int numOfPlayers)
    {
        keyboardPlayersCount = numOfPlayers;
        //set global var
        Debug.Log("Number of players on keyboard set to: " + numOfPlayers);
    }

    public GameObject DeviceIconPrefab;
    public void PlayerAdded(PlayerInput playerInput)
    {
        GameManager.instance.AddPlayer(playerInput);
        //add icon to grid
        GameObject deviceIcon = Instantiate(DeviceIconPrefab, transform.parent.Find("Grid"));
        deviceIcon.GetComponent<Image>().material.SetColor("_TargetColor", GameManager.instance.playerInfoList[GameManager.instance.playerInfoList.Count-1].colour);
    }

    //-------------------------------- BUTTONS ------------------------------
    public void playGame()
    {
        if (keyboardPlayersCount == 2)
        {
            GameManager.instance.AddPlayer("keyboard1", Keyboard.current);
        }
        GameManager.instance.AddPlayer("keyboard2", Keyboard.current);
        SceneManager.LoadScene("Intermission");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
