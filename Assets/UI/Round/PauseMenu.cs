using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUIObject;
    public InputActionAsset IAA;

    public bool isPaused = false;

    public void Init()
    {
        PlayerInput pi = gameObject.AddComponent<PlayerInput>();
        pi.actions = IAA;
        pi.defaultActionMap = "UI";
        pi.enabled = false;
        pi.enabled = true;
    }

    public void OnPause(InputValue input)
    {
        AudioManager.instance.PlayButtonSfx();
        Pause();
    }

    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused && !RoundManager.instance.isGameOver)
        {
            //activate menu
            PauseMenuUIObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            PauseMenuUIObject.SetActive(false);
            Time.timeScale = 1f;    
        }

    }


    public void Quit()
    {
        Application.Quit();
    }


    public void ToMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
