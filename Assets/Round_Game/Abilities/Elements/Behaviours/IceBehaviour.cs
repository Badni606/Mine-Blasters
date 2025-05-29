using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceBehaviour : ScriptableObject, ElementScript
{
    public TileBase IceTile;

    Tilemap obstacleTilemap;

    public void applyPlacableEffect(Vector3 position)
    {
        if(obstacleTilemap == null)
        {
            obstacleTilemap = GameObject.Find("/Grid/Obstacles").GetComponent<Tilemap>();
        }

        obstacleTilemap.SetTile(new Vector3Int((int)position.x,(int)position.y,0), IceTile);
    }

    [Header("Projectile Effect")]
    public float FreezeTime = 3;
    public Sprite FreezeSprite;

    public void applyProjectileEffect(Player player)
    {
        IEnumerator executable = freezePlayer(player);
        player.StartCoroutine(executable);
    }

    IEnumerator freezePlayer(Player player)
    {
        float normalSpeed = player.speed;
        player.speed = 0;
        player.ApplySpriteBkgrd(FreezeSprite);
        yield return new WaitForSeconds(FreezeTime);
        player.speed = normalSpeed;
        player.ApplySpriteBkgrd();
    }
}
