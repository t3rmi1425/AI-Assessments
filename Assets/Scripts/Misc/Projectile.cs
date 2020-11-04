using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 10;
    private Rigidbody2D rb;
    [SerializeField]
    private float count = 0;
    private float startCount = 3;
    
    public float dmg = 10;

    /// <summary>
    /// determines the source that the bullet comes from
    /// so whoever fires the shot doesnt destroy themselves
    /// </summary>
    public GameObject source;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        count = startCount; // starts the timer
    }

    private void Update()
    {
        // sets the bullets speed and the direction it will travel
        rb.velocity = transform.up * speed;

        // ticks the timer down
        count -= Time.deltaTime;
        // if the timer reaches 0 bullet will destroy itself
        if (count <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the sure is from gameObject
        if(source == collision.gameObject)
        {
            //return null
            return;
        }

        // if collision isnt null
        if (collision != null)
        {
            // destroy the projectile
            Destroy(this.gameObject);
        }
    
        // defines the player 
        PlayerController player = collision.gameObject.GetComponent < PlayerController >();
        
        // if the player is hit
        if (player != null)
        {
            // deal 10 damage
            player.Damage(10);
        }
    }
}
