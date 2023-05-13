using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBouncer : MonoBehaviour
{
    public float amplitude = 1.0f;
    public float frequency = 1.0f;

    Vector3 positionOffset = new Vector3();
    Vector3 updatedPosition = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        positionOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        updatedPosition = positionOffset;
        updatedPosition.y += amplitude * Mathf.Sin(Time.fixedTime * Mathf.PI * frequency);

        transform.position = updatedPosition;
    }
}
