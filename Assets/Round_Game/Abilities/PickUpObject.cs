using UnityEngine;

public class PickUpObject
{
    public string ObjectName;
    public PICKUP_TYPE PickUpType;

    //If weapon must contain elemnt
    public Weapon Weapon;
    public Element Element;
    public Effect Effect;

    public Player OriginPlayer;

    public Sprite PickUpSprite;
    //public GameObject ObjectPrefab;

    public PickUpObject(Effect effect)
    {
        PickUpType = PICKUP_TYPE.EFFECT;
        PickUpSprite = effect.sprite;
        Effect = effect;
    }

    public PickUpObject(Weapon weapon,Element element)
    {
        PickUpType = PICKUP_TYPE.WEAPON;
        Weapon = weapon;
        Element = element;
        PickUpSprite = weapon.sprite;
    }



}


public enum PICKUP_TYPE
{
    WEAPON,
    EFFECT
}


