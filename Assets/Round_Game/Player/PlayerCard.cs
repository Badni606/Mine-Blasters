using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Image HeldItem;
    public TextMeshProUGUI healthCounter;
    public RectTransform overlay;
    public GameObject DeathIndicator;

    public void UpdateHealth(int newValue)
    {
        healthCounter.text = newValue.ToString();
        if(newValue <= 0)
        {
            DeathIndicator.SetActive(true);
        }
    }


    public void setHeldItem(Sprite baseSprite,Color Tint)
    {
        HeldItem.enabled = true;
        HeldItem.sprite = baseSprite;
        HeldItem.color = Tint;
    }

    public void setHeldItem()
    {
        HeldItem.enabled = false;
    }

    public void Init(float mineCooldown,Color playerColour)
    {
        this.mineCooldown = mineCooldown;
        GetComponent<Image>().color = playerColour;

    }
    float mineCooldown = 5f;
    float currentMineCooldown;
    bool canMine = true;
    public void StartMineCooldown()
    {
        canMine = false;
        currentMineCooldown = mineCooldown;
    }

    void Update()
    {
        if (!canMine)
        {
            currentMineCooldown = currentMineCooldown - Time.deltaTime;
            float scale = currentMineCooldown / mineCooldown;
            if(scale < 0) 
            {
                scale = 0;
                canMine = true;
            }
            overlay.localScale = new Vector3 (1, scale, 1);
        }
    }


}
