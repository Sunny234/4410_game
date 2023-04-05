using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Destroy self after 0.5 seconds
    private void Awake() {
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter(Collision col) {
        // If collided object is NOT an enemy, only destroy self
        if(!(col.gameObject.CompareTag("Enemy"))) 
        {
            Destroy(gameObject);
        }

        // If collided object is an enemy, destroy self and the enemy
        if(col.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            col.gameObject.SetActive(false);
        }
    }
}