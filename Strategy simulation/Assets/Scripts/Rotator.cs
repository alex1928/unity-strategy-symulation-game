using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float xMin = -100;
    public float xMax = 100f;

    public float yMin = -100;
    public float yMax = 100f;

    public float zMin = -100;
    public float zMax = 100f;

    private Vector3 rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
        rotationSpeed = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), Random.Range(zMin, zMax));
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 speed = rotationSpeed * Time.deltaTime;

        transform.Rotate(speed.x, speed.y, speed.z);
    }
}
