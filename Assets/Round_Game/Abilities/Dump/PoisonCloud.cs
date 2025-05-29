using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoisonCloud : MonoBehaviour
{
    public float Duration;
    public int Damage;
    public float DamageInterval;
    //need to keep track of individual's times in lava
    [HideInInspector]
    public static Dictionary<GameObject, float> playerTimers = new Dictionary<GameObject, float>();


    private void Start()
    {
        //make shit move
        
        //destory self after duration 
        Destroy(gameObject, Random.Range(Duration - 0.3f, Duration + 0.3f));
    }

    private void FixedUpdate()
    {
        
    }

    public Tilemap Walls;
    public Tilemap Obstacles;
    void checkAdjacent()
    {
        Vector2 position = transform.position;
        for(int i = (int)position.x - 1 ;i<=(int)position.x + 1; i += 2)
        {

        }
        for (int i = (int)position.y - 1; i <= (int)position.y + 1; i += 2)
        {

        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        GameObject player = collision.gameObject;
        if (!playerTimers.ContainsKey(player))
        {
            playerTimers[player] = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        GameObject player = collision.gameObject;
        playerTimers[player] += Time.deltaTime;
        if (playerTimers[player] >= DamageInterval)
        {
            collision.gameObject.GetComponent<Player>().TakeDamange(Damage);
            playerTimers[player] = 0f; // Reset timer after dealing damage
        }

    }
}
