using UnityEngine;

public class HealEffect : ScriptableObject, EffectScript
{
    public int MaxHeal;
    public int MinHeal;
    public void ApplyEffect(Player player)
    {
        int healAmount = Random.Range(MinHeal, MaxHeal+1);
        player.Heal(healAmount);
    }
}
