using UnityEngine;

[CreateAssetMenu(fileName = "PoisonBehaviour", menuName = "Scriptable Objects/PoisonBehaviour")]
public class PoisonBehaviour : ScriptableObject, ElementScript
{
    [Header("Placeable Effects")]
    public GameObject GasPrefab;
    [Min(0)]
    public int GasDuration;
    [Min(0)]
    public int GasDamage;
    [Min(0)]
    public float GasDamageInterval;

    public void applyPlacableEffect(Vector3 position)
    {
        GameObject poisonCloud = Instantiate(GasPrefab, position, Quaternion.identity);
        poisonCloud.GetComponent<PoisonCloud>().Duration = GasDuration;
        poisonCloud.GetComponent<PoisonCloud>().Damage = GasDamage;
        poisonCloud.GetComponent<PoisonCloud>().DamageInterval = GasDamageInterval;
    }

    public void applyProjectileEffect(Player player)
    {
        throw new System.NotImplementedException();
    }
}
