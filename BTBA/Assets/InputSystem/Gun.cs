using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Gun : MonoBehaviour
{
    public StarterAssetsInputs input;
    public GameObject bulletPrefab;
    public GameObject bulletPoint;
    public float bulletSpeed = 1000;

    private GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        input = transform.root.GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.shoot)
        {
            Shoot();
            input.shoot = false;
        }
    }

    void Shoot()
    {
        bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);        
    }

}