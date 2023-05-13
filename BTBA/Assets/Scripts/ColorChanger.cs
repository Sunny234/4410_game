using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color newColor;
    private float blueRatio;
    public float frequency = 1;

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = transform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // This will range between 0 and 1
        blueRatio = Mathf.Abs(Mathf.Cos(Time.fixedTime * frequency));

        newColor = new Color(0f, 1f, blueRatio, 0.43f);

        objectRenderer.material.color = newColor;
    }

}