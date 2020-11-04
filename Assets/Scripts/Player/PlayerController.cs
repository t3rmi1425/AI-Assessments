using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Requires the object to have a rigidbody before the script can be added
public class PlayerController : MonoBehaviour
{
    #region Variables

    #region controls
    [Header("MovementControls")]
    [SerializeField]
    private string up = "w";           
    [SerializeField]
    private string down = "s";          
    [SerializeField]
    private string leftRotate = "q";   
    [SerializeField]
    private string rightRotate = "e";   
    [SerializeField]
    private string strafeLeft = "a";
    [SerializeField]
    private string strafeRight = "d";
    #endregion

    [Space]

    [Header("SpeedControls")]
    [SerializeField]
    private float speed;                // variable for adjusting the players speed
    [SerializeField]
    private float rotationalSpeed;      // variable for adjusting rotational speed

    private bool death;                 // true or false player death
    public GameObject deathMenu;        
    
    [Space]

    private Rigidbody2D rb;              // variable that defines rb as rigidbody2D
    public float health;
    public GradientHealth playerHealth;
    public Shoot bam;                    // variable for calling the shoot script
    #endregion

    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();   // gets the component attached to the player called rigidbody2d
        death = false;                      // makes sure the player doesnt start dead
    }

    private void Update()
    {
        // calls the player health method
        PlayerHealth();

        // if death is true then the player has died
        if (death == true)
        {
           deathMenu.SetActive(true); 
        }

        // if key is pressed the player will fire
        if (Input.GetKeyDown(KeyCode.P))
        {
            bam.FireWhenReady();
        }
    }

    /// <summary>
    /// For debuging
    /// </summary>
    /// <param name="damage">will do a set amount of damage to the player</param>
    public void Damage(int damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Determines if the player is alive and sets current health
    /// </summary>
    public void PlayerHealth()
    {  
        if (playerHealth != null)
        {
            playerHealth.currentHealth = health;
        }

        //health -= damage;
        if(health == 0)
        {
            Time.timeScale = 0;
            death = true;
        }
    }


    void FixedUpdate()
    {
        Movement();

        RotatePlayer();

    }

    #region Movement
    /// <summary>
    /// Method for storing all the players movement controls
    /// depending on the key pressed the player will be moved in the corresponding direction
    /// </summary>
    private void Movement()
    {
        if (Input.GetKey(up))
        {
            rb.velocity = transform.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(down))
        {
            rb.velocity = -transform.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(strafeLeft))
        {
            rb.velocity = -transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(strafeRight))
        {
            rb.velocity = transform.right * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// method for rotating the player when certain key is pressed
    /// </summary>
    private void RotatePlayer()
    {
        if (Input.GetKey(rightRotate))
        {
            rb.transform.Rotate(0, 0, -rotationalSpeed);
        }

        if (Input.GetKey(leftRotate))
        {
            rb.transform.Rotate(0, 0, rotationalSpeed);
        }
    }
    #endregion
}
