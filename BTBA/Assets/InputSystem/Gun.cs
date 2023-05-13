using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Gun : MonoBehaviour
{
    public StarterAssetsInputs input;
    public FirstPersonController controller;
    public GameObject bulletPrefab;
    public GameObject bulletPoint;
    public float bulletSpeed = 5000;

    private GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        input = transform.root.GetComponent<StarterAssetsInputs>();
        controller = transform.root.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.shoot && controller.canShoot && !(controller.isPaused))
        {
            Shoot();
            input.shoot = false;
        }
    }

    void Shoot()
    {
        bullet = Instantiate(bulletPrefab, bulletPoint.transform.position, transform.rotation);
        
        /* NOTE!: using '-transform.forward' b/c of weapon prefab default rotations*/
        bullet.GetComponent<Rigidbody>().AddForce(-transform.forward * bulletSpeed);        
    }

}