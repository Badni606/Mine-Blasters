using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{

    //Movement vars
    [Header("Movement")]
    public float MaxSpeed = 5.0f;

    [HideInInspector]
    public float speed = 5.0f;

    public Animator animator;

    Rigidbody2D rb;
    Vector2 movementDirection;
    Vector2 lastDirection = Vector2.down; //to be used in ability firing
    string lastDirectionName = "Down";

    //ability vars
    [Header("Abilties")]
    public static float HitCooldown = 5f;
    [HideInInspector]
    public Tilemap WallTileMap;
    [HideInInspector]
    public Tilemap ObstacleTileMap;

    float firing = 0f;//kind of unneccessary
    PickUpObject currentWeapon;

    //health
    [Header("Status")]
    public int health = 10;
    public PlayerCard PlayerCard;
    SpriteRenderer spriteRenderer;
    Color playerColour;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerColour = spriteRenderer.material.GetColor("_TargetColor");

        WallTileMap = GameObject.Find("/Grid/Walls").GetComponent<Tilemap>();
        ObstacleTileMap = GameObject.Find("/Grid/Obstacles").GetComponent<Tilemap>();
        speed = MaxSpeed;
    }

    //----------------------------------- ABILITIES -------------------------------------

    public void OnFire(InputValue input)
    {
        firing = input.Get<float>();
        if (firing == 1f && currentWeapon != null)
        {
            //check for wall and obstacle tile
            Vector3Int posToCheck = new Vector3Int((int)transform.position.x + (int)lastDirection.x, (int)transform.position.y + (int)lastDirection.y,0);

            TileBase wall = WallTileMap.GetTile(posToCheck);
            TileBase obstacle = ObstacleTileMap.GetTile(posToCheck);

            if(wall != null)
            {
                return;
            }

            //spawn in object in the next cell
            if(currentWeapon.PickUpType == PICKUP_TYPE.WEAPON)
            {
                WeaponScript weapon;
                Quaternion rotation = Quaternion.Euler(0,0,Mathf.Rad2Deg * Mathf.Atan2(lastDirection.y,lastDirection.x)+90);

                switch (currentWeapon.Weapon.WeaponType)
                {
                    case WEAPON_TYPE.PLACEABLE:
                        if(obstacle != null)
                        {
                            return;
                        }
                        weapon = Instantiate(currentWeapon.Weapon.prefab,posToCheck+new Vector3(0.5f,0.5f,0f),Quaternion.identity).GetComponent<WeaponScript>();
                        break;
                    case WEAPON_TYPE.PROJECTILE:
                        weapon = Instantiate(currentWeapon.Weapon.prefab, transform.position + new Vector3(lastDirection.x *0.1f , lastDirection.y * 0.1f, 0f), rotation).GetComponent<WeaponScript>();
                        break;
                    default:
                        weapon = Instantiate(currentWeapon.Weapon.prefab, posToCheck + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity).GetComponent<WeaponScript>();
                        break;
                }
                weapon.PickUpObject = currentWeapon;
                currentWeapon = null;
                PlayerCard.setHeldItem();
            }

        }
    }

    public AudioClip MineSFX;
    bool canHit = true;
    public void OnHit(InputValue input)
    {
        if (input.Get<float>() == 1f && canHit)
        {
            Vector3Int posToCheck = new Vector3Int((int)transform.position.x + (int)lastDirection.x, (int)transform.position.y + (int)lastDirection.y, 0);
            TileBase obstacle = ObstacleTileMap.GetTile(posToCheck);
            if (obstacle != null)
            {
                canHit = false;
                ObstacleTileMap.SetTile(posToCheck, null);
                PlayerCard.StartMineCooldown();
                Invoke(nameof(resetHitCooldown), HitCooldown);
                AudioManager.instance.PlaySFX(MineSFX);
            }
        }
    }

    void resetHitCooldown()
    {
        canHit = true;
    }

    //----------------------------------- MOVEMENT -----------------------------------------
    
    public void OnMove(InputValue input)
    {
        movementDirection = getDirection(input.Get<Vector2>().normalized);
    }

    public bool PickUpAbility(PickUpObject pickUp)
    {

        switch (pickUp.PickUpType)
        {
            //weapons need a slot to go into
            case PICKUP_TYPE.WEAPON:
                if (currentWeapon == null)
                {
                    currentWeapon = pickUp;
                    currentWeapon.OriginPlayer = this;
                    PlayerCard.setHeldItem(pickUp.Weapon.sprite, pickUp.Element.Color);
                    return true;
                }
                else
                {
                    return false;
                }
            //effects should fire off immediatley
            case PICKUP_TYPE.EFFECT:
                EffectScript effect = (EffectScript)pickUp.Effect.EffectScript;
                effect.ApplyEffect(this);
                return true;
            default:
                return false;
        }
    }

    void Update()
    {
        handleAnims();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementDirection * speed;
    }

    private Vector3 getDirection(Vector3 input)
    {
        Vector3 finalDirection = Vector2.zero;
        if (input.y > 0.01f)
        {
            lastDirectionName = "Up";
            finalDirection = new Vector2(0, 1);
            lastDirection = finalDirection;
        }
        else if (input.y < -0.01f)
        {
            lastDirectionName = "Down";
            finalDirection = new Vector2(0, -1);
            lastDirection = finalDirection;
        }
        else if (input.x > 0.01f)
        {
            lastDirectionName = "Right";
            finalDirection = new Vector2(1, 0);
            lastDirection = finalDirection;
        }
        else if (input.x < -0.01f)
        {
            lastDirectionName = "Left";
            finalDirection = new Vector2(-1, 0);
            lastDirection = finalDirection;
        }
        else
            finalDirection = Vector2.zero;

        return finalDirection;
    }

    void handleAnims()
    {
        string animationName = "";

        if (movementDirection == Vector2.zero)
            animationName = "Idle";
        else
            animationName = "Walking";

        animator.Play(animationName + lastDirectionName);
    }


    //-------------------------------------HEALTH----------------------------------------------------------
    [Header("Health")]
    public float DamageIndicatoreDuration;
    public AudioClip DamageSFX;
    bool isDead = false;

    public void TakeDamange(int damage)
    {
        if (RoundManager.instance.isGameOver || isDead)
        {
            return;
        }
        //play damage aniumation (could be a red highlight)
        spriteRenderer.material.SetColor("_TargetColor", Color.red);
        //play sfx
        AudioManager.instance.PlaySFX(DamageSFX);
        Invoke(nameof(normaliseColour), DamageIndicatoreDuration);
        health -= damage;
        PlayerCard.UpdateHealth(health);

        if(health <= 0)
        {
            isDead = true;
            RoundManager.instance.RegisterDeath(GetComponent<PlayerInput>().playerIndex);
            //play death animation

            //destory
            Destroy(gameObject);
        }

    }

    public AudioClip HealSFX;
    public void Heal(int amount)
    {
        if (RoundManager.instance.isGameOver || isDead)
        {
            return;
        }
        //play damage aniumation (could be a red highlight)
        spriteRenderer.material.SetColor("_TargetColor", Color.green);
        //play sfx
        AudioManager.instance.PlaySFX(HealSFX);
        Invoke(nameof(normaliseColour), DamageIndicatoreDuration);
        health += amount;
        PlayerCard.UpdateHealth(health);
    }

    void normaliseColour()
    {
        spriteRenderer.material.SetColor("_TargetColor",playerColour);
    }

    //-----------------------------------------------------------------------------------------------------------
    [Header("Utility")]
    public SpriteRenderer ForegroundSpriteRenderer;
    public void ApplySpriteBkgrd(Sprite sprite)
    {
        ForegroundSpriteRenderer.sprite = sprite;
        ForegroundSpriteRenderer.color = Color.white;
    }

    public void ApplySpriteBkgrd()
    {
        ForegroundSpriteRenderer.color = new Color(1, 1, 1, 0);
    }

}
