using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 0.2f;

    private void Awake() {
        Destroy(gameObject, life);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            Destroy(gameObject);
            other.gameObject.SetActive(false);
        }
        else {
            Destroy(gameObject, life);
        }
    }

    private void OnCollisionEnter(Collision col) {
        if(!(col.gameObject.CompareTag("Enemy"))) {
            Destroy(gameObject);
        }
    }
}