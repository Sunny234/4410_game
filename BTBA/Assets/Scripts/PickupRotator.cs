using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRotator : MonoBehaviour
{
    public float degreesPerSecond_x;
    public float degreesPerSecond_y;
    public float degreesPerSecond_z;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(degreesPerSecond_x, degreesPerSecond_y, degreesPerSecond_z) * Time.deltaTime);    
    }
}
