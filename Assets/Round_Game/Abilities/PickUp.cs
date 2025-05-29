using UnityEngine;

public class PickUp : MonoBehaviour
{
    public AudioClip PickUpSFX;
    PickUpObject pickUpObject;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        //Randomly assign self ability
        pickUpObject = RoundManager.instance.GeneratePickUp();
        //Display corresponding sprite
        sr.sprite = pickUpObject.PickUpSprite;
        if(pickUpObject.PickUpType == PICKUP_TYPE.WEAPON)
        {
            sr.color = pickUpObject.Element.Color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if player, send them ability data, else destroy
        if(collision.CompareTag("Player")) 
        {
            if (collision.gameObject.GetComponent<Player>().PickUpAbility(pickUpObject))
            {
                AudioManager.instance.PlaySFX(PickUpSFX);
                Destroy(gameObject);
                RoundManager.instance.PickUpPicked();
            }
        }
        else //not being called for some reason?????????????????
        {
            DestoryPickUp();
        }
    }

    public void DestoryPickUp()
    {
        RoundManager.instance.PickUpPicked();
        Destroy(gameObject);
    }

}