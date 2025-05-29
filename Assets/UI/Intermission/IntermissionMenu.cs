using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionMenu : MonoBehaviour
{
    public void StartRound()
    {
        SceneManager.LoadScene("Round_Game");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
