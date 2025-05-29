using System.Collections;
using UnityEngine;

public class SpeedUpEffect : ScriptableObject, EffectScript
{
    [Min(0)]
    public float EffectDuration;
    [Min(1)]
    public float EffectMultiplier;


    public void ApplyEffect(Player player)
    {
        IEnumerator effect = speedUp(player);
        player.StartCoroutine(effect);
    }

    IEnumerator speedUp(Player player)
    {
        player.speed = player.MaxSpeed *EffectMultiplier;
        //could potentiually add foregorund image of an arrow or some shit
        yield return new WaitForSeconds(EffectDuration);
        player.speed = player.MaxSpeed;
    }
}

