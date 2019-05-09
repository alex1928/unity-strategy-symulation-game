using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float moveSpeed = 20f;
    public float rotationSpeed = 100f;
    public float rotationSmoothing = 10f;

    public Camera cam;

    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float rotation = 0;

        if(transform.position.y <= 10 && scroll < 0)
            scroll = 0;
        
        if(transform.position.y >= 50 && scroll > 0)
            scroll = 0;
        
        if(Input.GetKey(KeyCode.Q))
            rotation = rotationSpeed * Time.deltaTime;
        
        if(Input.GetKey(KeyCode.E))
            rotation = -rotationSpeed * Time.deltaTime;

        Vector3 moveVector = transform.forward * vertical + transform.right * horizontal + transform.up * scroll * 2;
        Vector3 targetPosition = transform.position + moveVector * moveSpeed * Time.deltaTime;

        cam.transform.position += -transform.forward * scroll * moveSpeed * Time.deltaTime * 2;
        
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + rotation, targetRotation.eulerAngles.z);
        
        transform.position = targetPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing * Time.deltaTime);
    }
}
