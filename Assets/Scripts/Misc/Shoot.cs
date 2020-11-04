using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform bullet;
    public Transform spawner;
    public AudioSource shoot;

    /// <summary>
    /// Spawns a bullet at the spawners position and rotation
    /// </summary>
    public void FireWhenReady()
    {
        shoot.Play();
        Transform transform = Instantiate(bullet, spawner.position, spawner.rotation);
        
        // sets the source to whatever gameobject fires the bullet
        Projectile _bullet = transform.GetComponent<Projectile>();
        _bullet.source = gameObject;
    }
}
