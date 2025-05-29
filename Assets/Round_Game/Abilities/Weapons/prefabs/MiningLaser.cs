using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningLaser : MonoBehaviour, WeaponScript
{
    [Header("Blast Properties")]
    [Min(0)]
    public int laserRange;
    [Min(0)]
    public int Damage = 4;
    [Min(0)]
    public float Duration;
    public float LaserDelay = 0f;

    [Header("Effects")]
    public AudioClip laserFireSFX;

    PickUpObject pickUpObject;
    SpriteRenderer spriteRenderer;
    GameObject firePoint;

    ElementScript element;
    BoxCollider2D damageCollider;

    Tilemap ObstacleTileMap;
    Tilemap GroundTileMap;

    public PickUpObject PickUpObject
    {
        get { return pickUpObject; }
        set { pickUpObject = value; }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        applyElementColour();
        ObstacleTileMap = GameObject.Find("/Grid/Obstacles").GetComponent<Tilemap>();
        shooter = pickUpObject.OriginPlayer;

        element = (ElementScript)pickUpObject.Element.BehaviourScript;
        firePoint =  transform.GetChild(0).gameObject;
        damageCollider = GetComponent<BoxCollider2D>();
        //possibility to animate a charge up or smthn
        Invoke(nameof(fire), LaserDelay);
    }

    public void applyElementColour()
    {
        spriteRenderer.color = pickUpObject.Element.Color;
    }

    void fire()
    {
        //make box collider 
        damageCollider.size = new Vector2(0.95f, laserRange);
        damageCollider.isTrigger = true;
        damageCollider.offset = new Vector2(0,(-laserRange/2f) -0.5f);//need to figure this one 

        //add visual 
        firePoint.transform.localScale = new Vector3(laserRange,1,1);
        firePoint.transform.localPosition = new Vector3(0, (-laserRange / 2f) - 0.5f,0);


        //delete grid blocks
        Vector3 start = transform.TransformPoint(new Vector3(0,-0.5f,0));//local firepoint origin
        Vector3 end = transform.TransformPoint(new Vector3(0,-laserRange-0.5f,0));//local tip of laser
        Vector3 line = end-start;

        Collider2D[] pickUps = Physics2D.OverlapAreaAll(transform.TransformPoint(new Vector3(-0.48f, -0.5f, 0)), transform.TransformPoint(new Vector3(0.48f, -laserRange - 0.5f, 0)),LayerMask.GetMask("PickUps"));
        foreach(Collider2D collider in pickUps)
        {
            collider.GetComponent<PickUp>().DestoryPickUp();
        }

        for(int i = 1;i<=laserRange;i++)
        {
            float scaleFactor = (laserRange - i) / (float)laserRange;
            
            Vector3 scaled = Vector3.Scale(line, new Vector3(scaleFactor, scaleFactor));
            var cellPos = ObstacleTileMap.WorldToCell(start + scaled);
            ObstacleTileMap.SetTile(cellPos, null);
        }

        AudioManager.instance.PlaySFX(laserFireSFX);
        Destroy(gameObject, Duration);
    }

    Player shooter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if(player == shooter)
            {
                return;
            }
            player.TakeDamange(Damage);
            //apply elemnt effect here
            element.applyProjectileEffect(player);
        }
    }


}
