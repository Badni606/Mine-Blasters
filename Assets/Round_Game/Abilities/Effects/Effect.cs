using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Objects/Effect")]
public class Effect : ScriptableObject
{
    public string Name;
    public Sprite sprite;
    //public EffectScript script;
    public ScriptableObject EffectScript;
}

public interface EffectScript
{
    public void ApplyEffect(Player player);
}
