using System.Collections;
using UnityEngine;

public class AiStates : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private float player_distance = 10f;    // variable for detecting if player is in sight of the AI
    [SerializeField]
    private float stop_distance = 5f;   // the distance the AI will stop at for when shooting at the Player
    [SerializeField]
    private float shooting_range = 2f;
    public Transform player;
    public Shoot fire;              // use to call the shotting script

    [SerializeField]
    private float c = 0;        // timer
    [SerializeField]
    private float sC = 5;       // variable to reset the timer

    public Vector2 ai;
    //public Transform enemy;

    public float speed;


    public GameObject[] waypoints;
    public float minDistance;
    public int index = 0;

    public AudioSource enemySighted;
    [SerializeField]
    private GradientHealth enemyHealth;
    public float health;
    #endregion


    #region State Machine
    // The States that the AI can go into
    public enum State
    {
        patrol,
        seek,
        attack,
        retreat,
    }

    public State state;

    /// <summary>
    /// While the AI is in the patrol state it will follow Set Waypoints unless the AI detects the player
    /// </summary>
    private IEnumerator patrolState()
    {   
        while (state == State.patrol)
        {
            if (Vector2.Distance(player.position, transform.position) > player_distance)    // if player is far away from the enemy
            {
                state = State.patrol;                                                       // then state = patrol
                Patrol();
            }
            else
            {                                                                               // else
                enemySighted.Play();
                state = State.seek;                                                         // then state = seek
            }

            // will make sure the enemy goes into the retreat state when health is below or equal to 20
            if (health <= 20.5f)
            {
                state = State.retreat;
            }

            yield return null;
        }
        NextState();
    }
    
    /// <summary>
    /// If the AI detects the player it will move into the Seek State and follow the player
    /// if the AI is close to the player it'll stop at a set distance and enter the attack state
    /// </summary>
    private IEnumerator seekState()
    {
        Debug.Log("Seeking Player");
        while (state == State.seek)
        {
            if (Vector2.Distance(player.position, transform.position) < player_distance)
            {
                
                TargetPlayer();

                if (Vector2.Distance(player.position, transform.position) <= stop_distance)
                {
                    Stop();
                    state = State.attack;
                }
            }

            else
            {
                state = State.patrol;
            }

            // will make sure the enemy goes into the retreat state when health is below or equal to 20
            if (health <= 20)
            {
                state = State.retreat;
            }

            yield return null;
        }
        NextState();
    }

    /// <summary>
    /// While in the attack state the enemy will open fire on the player
    /// </summary>
    private IEnumerator attackState()
    {
        c = sC;

        while (state == State.attack)
        {
            c -= Time.deltaTime;

            if (Vector2.Distance(player.position, transform.position) <= shooting_range)
            {
                if (c <= 0)
                {
                    fire.FireWhenReady(); // used to shoot the player
                    c = sC;
                }
                Debug.Log("Shooting");
            }

            if (Vector2.Distance(player.position, transform.position) <= stop_distance)
            {
                Stop();
                if (c <= 0)
                {
                    fire.FireWhenReady(); // used to shoot the player
                    c = sC;
                }
                    Debug.Log("Shooting");
            }

            else if (Vector2.Distance(player.position, transform.position) >= stop_distance)
            {
                state = State.seek;
                Debug.Log("Seeking");
            }
            else
            {
                c = sC;
            }
            // will make sure the enemy goes into the retreat state when health is below or equal to 20
            if (health <= 20)
            {
                 state = State.retreat;
            }

            yield return null;
        }
        NextState();
    } 
   
    /// <summary>
    /// When the AI goes below 20% health it'll enter retreat and run away from the player
    /// </summary>
    private IEnumerator retreatState()
    {
        while (state == State.retreat)
        {
            Retreat();

            health += Time.deltaTime * 2;
            if(health >= 50)
            {
                health = 50;
                state = State.patrol;
            }
            yield return null;
        }

        NextState();
    }

    private void Start()
    {
        NextState();
    }

    /// <summary>
    /// When called will go to the next state
    /// </summary>
    private void NextState()
    {
        //work out the name of the method we want to run
        string methodName = state.ToString() + "State"; // if the current state is patrol this returns patrol state and so on
        // give us a variable so we can run a method using its name
        System.Reflection.MethodInfo info = 
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        //run our method
        StartCoroutine((IEnumerator)info.Invoke(this, null));
        //Using StartCorountine() means we can leave and come back to the method that is running
        //All corountines must return IEnumerator
    }
    #endregion


    #region
    private void Update()
    {
        EnemyHealth();

        // if the AI goes below zero health it dies and is destroyed
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Damage(int damage)
    {
        // for debuging only
        health -= damage;
    }

    /// <summary>
    /// method for setting the AIs health 
    /// </summary>
    public void EnemyHealth()
    {
        if (enemyHealth != null)
        {
            enemyHealth.currentHealth = health;
        }
    }

    /// <summary>
    /// Gizmo for Seeing the AI's sight range
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, player_distance);
    }

    /// <summary>
    /// Gizmo for the AI's weapon range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, shooting_range);
    }
    #endregion   


    #region AImovement
    /// <summary>
    ///  The AI will follow a set path using waypoints
    /// </summary>
    void Patrol()
    {
        // defines the float varible distance
        float distance = Vector2.Distance(transform.position, waypoints[index].transform.position);
        
        // if AI reaches minDistance to the waypoint
        if (distance < minDistance)
        {
            // Increase index by 1
            // index is a list of waypoints
            index++;
        }

        // if index is greater or equal to the waypoint lenght 
        if(index >= waypoints.Length)
        {
            // reset index and go back to the first waypoint
            index = 0;
        }

        // moves the AI towards the current waypoint in the index
        MoveAi(waypoints[index].transform.position);
    }

    /// <summary>
    ///  Method used for moving the AI towards waypoints
    ///  as well as rotate the AI to look in the direction it is moving
    /// </summary>
    /// <param name="targetPosition">The current Waypoint's position</param>
    void MoveAi(Vector2 targetPosition)
    {
        // rotates the AI
        Vector3 relative = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);

        // Moves the AI
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    /// <summary>
    /// Method for targeting the player and 
    /// moving towards them
    /// </summary>
    private void TargetPlayer()
    {
        // rotates AI to look at player
        Vector3 relative = transform.InverseTransformPoint(player.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);

        // Moves AI towards Player
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    /// <summary>
    /// Stops the AI at a set distance from the player
    /// </summary>
    private void Stop()
    {
        // keeps the AI rotating to face the player when stopped
        Vector3 relative = transform.InverseTransformPoint(player.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);

        // stops the AI at a set distance from the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, 0);
    }

    /// <summary>
    /// Moves the AI away from the player
    /// </summary>
    private void Retreat()
    {
        // keeps the AI looking at the player as it moves away
        Vector3 relative = transform.InverseTransformPoint(player.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);

        // moves the AI away from the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
    }
    #endregion


    /// <summary>
    ///  Checks if the bullet has hit the AI
    ///  Then Does damage
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // detects what the colision is from
        Projectile doDamage;
        doDamage = collision.gameObject.GetComponent<Projectile>();

        // if doDamge
        if (doDamage != null)
        {
            // Ai takes set amount of damage
            health -= doDamage.dmg;
        }
    }
}
