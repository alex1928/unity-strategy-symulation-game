using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Storage))]
public class Collectable : StorageInteraction
{

    public bool isRawMaterial = false;
    public bool scaleWhenCollect = false;
    public float timeToCollect = 0;
    private float maxScale = 1f;
    private Vector3 targetScale;

    public int reservationCount = 0;


    public bool IsCollectionRequired {

        get {

            return storage.desiredAmount < storage.amount;
        }
    }



    public void Start()
    {
        
        maxScale = transform.localScale.x;
        targetScale = new Vector3(maxScale, maxScale, maxScale);
    }

    // Update is called once per frame
    public void Update()
    {

        if(scaleWhenCollect && transform.localScale != targetScale) {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 5 * Time.deltaTime);
        }
    }

    public bool ReserveForDrone() {

        if(reservationCount >= storage.amount)
            return false;

        reservationCount++;
        return true;
    }

    public void RemoveReservation() {

        reservationCount--;
    }

    public Material Collect() {

        Material mat = storage.Get();

        reservationCount--;

        if(isRawMaterial && storage.amount <= 0) {

            gameObject.SetActive(false);
            assignedTower.RemoveStorageInteraction(this);
            Destroy(gameObject, 0.1f);
            return mat;
        }

        if(scaleWhenCollect) {
            float percent = (float)storage.amount / (float)storage.maxAmount;
            float scale = maxScale * percent;
            targetScale = new Vector3(scale, scale, scale);
        }

        return mat;
    }


}
