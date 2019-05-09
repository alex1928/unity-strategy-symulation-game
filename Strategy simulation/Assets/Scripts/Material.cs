using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MaterialType {ConcreteOre, SteelOre, Steel, Cement, All}

[RequireComponent(typeof(Rotator))]
public class Material : MonoBehaviour
{

    public MaterialType type;

    public Vector3 targetPosition;
    private float moveSpeed = 15f;

    // Start is called before the first frame update
    void Start()
    {
        StopRotation();
        targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        if(Vector3.Distance(transform.localPosition, targetPosition) > 0.1) {

            Vector3 directionVector = (targetPosition - transform.localPosition).normalized;
            transform.localPosition += directionVector * moveSpeed * Time.deltaTime;
        }*/
    }

    public void SetCollectableActivity(bool active) {

        Collectable collectable = gameObject.GetComponent<Collectable>(); 
        if(collectable == null)
            return;

        collectable.enabled = active;
    }


    public void StartRotation() {

        GetComponent<Rotator>().enabled = true;
    }

    public void StopRotation() {

        GetComponent<Rotator>().enabled = false;
    }

    public void MoveTo(Vector3 target) {

        transform.localPosition = target;
        targetPosition = target;
    }
}
