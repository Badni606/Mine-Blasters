using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dynamite : MonoBehaviour,WeaponScript
{
    [Header("Blast Properties")]
    public float BlastTimer;
    public int BlastRadius;
    [Header("Effects")]
    public GameObject ExplosionPrefab;
    public AudioClip ExplosionSound;
    
    Tilemap ObstacleTileMap;
    Tilemap GroundTileMap;
    PickUpObject pickUpObject;
    ElementScript element;
    SpriteRenderer sr;

    public PickUpObject PickUpObject
    {
        get { return pickUpObject; }
        set { pickUpObject = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //load componenets and map vars
        element = (ElementScript)pickUpObject.Element.BehaviourScript;
        sr = GetComponent<SpriteRenderer>();

        ObstacleTileMap = GameObject.Find("/Grid/Obstacles").GetComponent<Tilemap>();
        GroundTileMap = GameObject.Find("/Grid/Ground").GetComponent<Tilemap>();

        applyElementColour();
        //change sprite
        //applyElementColour();
        //start timer
        Invoke(nameof(explode), BlastTimer);
    }
    
    public void applyElementColour()
    {
        sr.color = pickUpObject.Element.Color;
    }


    void explode()
    {
        Vector3Int centerCell = ObstacleTileMap.WorldToCell(transform.position); // Convert world pos to grid pos
        int range = Mathf.CeilToInt(BlastRadius); // Convert radius to tilemap range

        //blast the fuckers
        Collider2D[] playerCheck = Physics2D.OverlapCircleAll(((Vector2)transform.position), BlastRadius, LayerMask.GetMask("Players","PickUps"));
        foreach (Collider2D player in playerCheck)
        {
            if(player.TryGetComponent<Player>(out Player instance))
            {
                instance.TakeDamange(10);
            }
            else
            {
                player.gameObject.GetComponent<PickUp>().DestoryPickUp();
            }
        }



        //explosion sfx 
        AudioManager.instance.PlaySFX(ExplosionSound);

        //remove tiles and add effects
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int tilePos = centerCell + new Vector3Int(x, y, 0);

                // Convert tile position back to world position (center of the tile)
                Vector3 tileWorldPos = ObstacleTileMap.CellToWorld(tilePos) + ObstacleTileMap.cellSize / 2;

                // Check if the tile is inside the circular radius
                if (Vector3.Distance(tileWorldPos, transform.position) <= BlastRadius && GroundTileMap.HasTile(tilePos))
                {
                    //spawn explosion effect
                    GameObject effect = Instantiate(ExplosionPrefab, tileWorldPos, Quaternion.identity);
                    Destroy(effect,2f);
                    ObstacleTileMap.SetTile(tilePos, null); // Remove tile
                    //spawn element effect
                    element.applyPlacableEffect(tileWorldPos);
                }
            }
        }
        Destroy(gameObject);
    }

}
