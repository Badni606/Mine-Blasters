using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrossBomb : MonoBehaviour, WeaponScript
{
    [Header("Blast Properties")]
    public float BlastTimer;
    public int BlastRange;

    [Header("Effects")]
    public GameObject ExplosionPrefab;
    public AudioClip ExplosionSound;

    PickUpObject pickUpObject;
    SpriteRenderer spriteRenderer;

    Tilemap ObstacleTileMap;
    Tilemap GroundTileMap;


    ElementScript element;
    public PickUpObject PickUpObject
    {
        get { return pickUpObject; }
        set { pickUpObject = value; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ObstacleTileMap = GameObject.Find("/Grid/Obstacles").GetComponent<Tilemap>();
        GroundTileMap = GameObject.Find("/Grid/Ground").GetComponent<Tilemap>();

        element = (ElementScript)pickUpObject.Element.BehaviourScript;
        applyElementColour();
        Invoke(nameof(explode), BlastTimer);
    }

    public void applyElementColour()
    {
        spriteRenderer.color = pickUpObject.Element.Color;
    }

    void explode()
    {
        Vector3Int centerCell = ObstacleTileMap.WorldToCell(transform.position); // Convert world pos to grid pos

        //blast the fuckers
        Collider2D[] verticalRect = Physics2D.OverlapAreaAll(new Vector2(centerCell.x+0.05f,BlastRange+centerCell.y-0.05f), new Vector2(centerCell.x + 0.95f, centerCell.y -BlastRange+0.05f),LayerMask.GetMask("Players","PickUps"));
        Collider2D[] horizontalRect = Physics2D.OverlapAreaAll(new Vector2(centerCell.x -BlastRange+0.05f, centerCell.y+0.05f), new Vector2(centerCell.x + BlastRange - 0.05f, centerCell.y +0.85f), LayerMask.GetMask("Players","PickUps"));
        var playerSet = horizontalRect.Union(verticalRect).ToArray();
        foreach (Collider2D player in playerSet)
        {
            if(player.TryGetComponent(out Player instance))
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
        for (int x = -BlastRange; x <= BlastRange; x++)
        {
            if(x == 0)
            {
                explodeTile(centerCell, new Vector3Int(x, 0, 0));
                continue;
            }

            explodeTile(centerCell, new Vector3Int(x, 0, 0));
            explodeTile(centerCell, new Vector3Int(0, x, 0));
            
        }


        Destroy(gameObject);
    }

    void explodeTile(Vector3Int centerCell, Vector3Int offset)
    {
        Vector3Int tilePos = centerCell + offset;

        // Convert tile position back to world position (center of the tile)
        Vector3 tileWorldPos = ObstacleTileMap.CellToWorld(tilePos) + ObstacleTileMap.cellSize / 2;

        if(!GroundTileMap.HasTile(tilePos))
        {
            return;
        }
        //spawn explosion effect
        GameObject effect = Instantiate(ExplosionPrefab, tileWorldPos, Quaternion.identity);

        Destroy(effect, 2f);
        ObstacleTileMap.SetTile(tilePos, null); // Remove tile
        //spawn element effect
        element.applyPlacableEffect(tileWorldPos);
    }

}
