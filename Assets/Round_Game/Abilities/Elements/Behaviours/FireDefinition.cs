using System.Collections;
using UnityEngine;


public class FireDefinition : ScriptableObject, ElementScript
{
    [Header("Placeable Effects")]
    public GameObject FirePrefab;
    [Min(0)]
    public int FireDuration;
    [Min(0)]
    public int FireDamage;
    [Min(0)]
    public float FireDamageInterval;

    public void applyPlacableEffect(Vector3 position)
    {
        GameObject fire = Instantiate(FirePrefab,position,Quaternion.identity);
        fire.GetComponent<lava>().Duration = FireDuration;
        fire.GetComponent<lava>().Damage = FireDamage;
        fire.GetComponent<lava>().DamageInterval = FireDamageInterval;
    }

    [Header("Projectile Effects")]
    public int BurnDamage = 2;
    public int MaxBurns = 2;
    public float BurnIntervals = 2f;
    public Sprite BurnSprite;

    public void applyProjectileEffect(Player player)
    {
        IEnumerator executable = burn(player);
        player.StartCoroutine(executable);
    }

    IEnumerator burn(Player player)
    {
        int burns = 0;
        player.ApplySpriteBkgrd(BurnSprite);
        while(burns < MaxBurns)
        {
            player.TakeDamange(BurnDamage);
            burns++;
            yield return new WaitForSeconds(BurnIntervals);
        }
        player.ApplySpriteBkgrd();
        
    }


}
