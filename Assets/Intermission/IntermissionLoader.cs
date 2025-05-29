using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntermissionLoader : MonoBehaviour
{
    public GameObject PlayerScoreBarUI;
    public Transform PlayerScoreBarHolder;

    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;

    public GameObject StartButtonObject;


    private List<int> winnerIndex = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DescriptionText.text = $"First player to score {GameManager.instance.scoreToWin} points wins! Points can be earned by being the last standing in a round";
        //get round winner 
        //AND 
        //spawn playerScore UI 
        for(int i = 0; i < GameManager.instance.playerCount; i++)
        {
            GameObject entry = Instantiate(PlayerScoreBarUI,PlayerScoreBarHolder);
            //scanning for multiple winners incase of draws
            if (GameManager.instance.GetScore(i) == GameManager.instance.scoreToWin)
            {
                winnerIndex.Add(i);
            }
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "P"+(i+1);
            entry.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3((float)GameManager.instance.GetScore(i)/(float)GameManager.instance.scoreToWin,1,0);
            Image entryImage = entry.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
            entryImage.material = new Material(entryImage.material);
            entryImage.material.SetColor("_TargetColor", GameManager.instance.playerInfoList[i].colour);
        }

        checkWinner();
        //set animations for score increase(after an N sec wait)

    }


    void checkWinner()
    {

        if (winnerIndex.Count > 0)
        {
            TitleText.text = "Game Over";
            switch (winnerIndex.Count)
            {
                case 1:
                    DescriptionText.text = $"Player {winnerIndex[0] + 1} wins!";
                    StartButtonObject.SetActive(false);
                    break;
                case 2:
                    DescriptionText.text = $"Players {winnerIndex[0] + 1} and {winnerIndex[1] + 1} drew!";
                    StartButtonObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

    }
}
