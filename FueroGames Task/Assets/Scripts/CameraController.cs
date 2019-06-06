using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform playerTransform;
    public float cameraFollowSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = playerTransform.position;
        target.z = -10;

        transform.position = Vector3.Lerp(transform.position, target, cameraFollowSpeed * Time.deltaTime);
    }
}
