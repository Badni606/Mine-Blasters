using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string Name;
    public WEAPON_TYPE WeaponType;
    public Sprite sprite;
    public GameObject prefab;
}

public enum WEAPON_TYPE
{
    PLACEABLE,
    PROJECTILE,
    CONSUMABLE
}

public interface WeaponScript
{
    public PickUpObject PickUpObject {
        get; set;
    }

    public void applyElementColour();
}
