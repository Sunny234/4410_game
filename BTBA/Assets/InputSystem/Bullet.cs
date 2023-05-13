using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /* IMPORTANT NOTE! */
    /* 
        Fixed enemy bullets colliding with each other / other enemies by giving them an "Enemy" Layer,
        then disabling collisions between "Enemy" and "Enemy" in the Collision Matrix under 
        Edit > Project Settings > Physics
    */

    // Destroy self after 2.5 seconds
    private void Awake() {
        Destroy(gameObject, 2.5f);
    }

    // Upon a collision, destroy self
    private void OnCollisionEnter(Collision col) {
        Destroy(gameObject);
    }
}