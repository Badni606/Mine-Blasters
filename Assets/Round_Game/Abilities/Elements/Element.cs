using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Scriptable Objects/Element")]
public class Element : ScriptableObject
{
    public string Name;
    public Color Color;
    public WEAPON_TYPE[] CompatibleWeapons;
    public ScriptableObject BehaviourScript;
}

public interface ElementScript
{
    public void applyPlacableEffect(Vector3 position);

    public void applyProjectileEffect(Player player);
}
