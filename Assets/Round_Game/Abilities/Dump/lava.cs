using System.Collections.Generic;
using UnityEngine;

public class lava : MonoBehaviour
{

    public float Duration;
    public int Damage;
    public float DamageInterval;
    //need to keep track of individual's times in lava
    [HideInInspector]
    public static Dictionary<GameObject,float> playerTimers = new Dictionary<GameObject,float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, Random.Range(Duration - 0.3f, Duration + 0.3f));   
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
