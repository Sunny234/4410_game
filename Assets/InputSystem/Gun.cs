using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Gun : MonoBehaviour
{
    private StarterAssetsInputs input;
    [SerializeField] // Allows us to see bulletPrefab in the editor
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject bulletPoint;
    
    [SerializeField]
    private float bulletSpeed = 600;

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