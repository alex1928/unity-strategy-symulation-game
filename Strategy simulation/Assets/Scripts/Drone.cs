using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum DroneStatus {Free, GoingToCollect, GoingToDrop, GoingToCharge, Going, HoldingMaterial, Collecting, Charging}

public class Drone : MonoBehaviour
{
    NavMeshAgent agent;

    public GameObject targetObject;
    public Vector3 targetPosition;
    public DroneStatus status = DroneStatus.Free;

    public Material holdingMaterial = null;
    public Transform materialPlaceholder;

    public float timeToFinnishCollecting = 0;

    public float maxPower = 100;
    public float currentPower = 100;
    public float idleDischarge = .1f;
    public float moveDischarge = .5f;

    public float lowPower = 20;

    private float timeSinceStatusChange = 0;

    public bool NeedsCharge {

        get {

            return currentPower <= lowPower;
        }
    }

    void Start() {

        //Obsługa eventu gdy docelowy obiekt moze zostac usuniety
        DroneManager.OnObjectDelete += StopFollowingObject;

        agent = GetComponent<NavMeshAgent>();
        agent.destination = transform.position;

        StartCoroutine("UpdateNavigationAgent");
    }


    void Update() {

        float distance;
        Collectable collectable;

        switch(status) {

            case DroneStatus.GoingToCollect:

                distance = Vector3.Distance(transform.position, targetObject.transform.position);
                collectable = targetObject.GetComponent<Collectable>();
                if(collectable != null && distance <= collectable.range) {

                    timeToFinnishCollecting = collectable.timeToCollect;
                    SetStatus(DroneStatus.Collecting);
                    agent.destination = transform.position;
                }

                break;
            
            case DroneStatus.Collecting:

                timeToFinnishCollecting -= Time.deltaTime;

                if(timeToFinnishCollecting <= 0) {

                    collectable = targetObject.GetComponent<Collectable>();
                    if(collectable == null) {
                        SetStatus(DroneStatus.Free);
                        break;
                    }
                    Collect(collectable);
                }

                break;


            case DroneStatus.GoingToDrop:

                if(targetObject == null)
                    SetStatus(DroneStatus.HoldingMaterial);
                
                distance = Vector3.Distance(transform.position, targetObject.transform.position);
                Dropable dropable = targetObject.GetComponent<Dropable>();
                if(dropable != null && distance <= dropable.range) {

                    Drop(dropable);
                    agent.destination = transform.position;
                }

                break;

            case DroneStatus.GoingToCharge:

                if(targetObject == null)
                    SetStatus(DroneStatus.Free);
                
                distance = Vector3.Distance(transform.position, targetObject.transform.position);
                if(distance <= 2) {

                    SetStatus(DroneStatus.Charging);
                    agent.destination = transform.position;
                }
                break;

            case DroneStatus.Charging:

                currentPower += 5 * Time.deltaTime;
                if(currentPower >= maxPower) {
                    SetStatus(DroneStatus.Free);
                }
                break;

            case DroneStatus.Going:
                
                if(agent.remainingDistance <= 0.1) {
                    SetStatus(DroneStatus.Free);
                }

                break;


            case DroneStatus.HoldingMaterial:

                if(timeSinceStatusChange >= 3)
                    Drop();

                break;
                
        }

        Discharge();
        timeSinceStatusChange += Time.deltaTime;
    }

    public void SetStatus(DroneStatus status) {

        this.status = status;
        timeSinceStatusChange = 0;
    }

    void Discharge() {

        if(currentPower <= 0)
            return;
        
        float dischargeAmount = moveDischarge;

        if(status == DroneStatus.Charging)
            dischargeAmount = 0;
        
        if(status == DroneStatus.Free)
            dischargeAmount = idleDischarge;

        currentPower -= Time.deltaTime * dischargeAmount; 
    }


    IEnumerator UpdateNavigationAgent() {

        Vector3 target = Vector3.zero;

        while(true) {

            switch(status) {

                case DroneStatus.GoingToCollect:
                case DroneStatus.GoingToDrop:
                case DroneStatus.GoingToCharge:
                    target = targetObject.transform.position;
                    break;
                
                case DroneStatus.Going:
                    target = targetPosition;
                    break;

                default: 
                    yield return new WaitForSeconds(1);
                    continue;
            }

            target.y = transform.position.y;

            agent.destination = target;

            yield return new WaitForSeconds(1);
        }
    }

    public void Charge(Charger charger) {

        Collectable collectable = targetObject.GetComponent<Collectable>();
        if(status == DroneStatus.GoingToCollect && collectable != null) {
            collectable.RemoveReservation();
        }

        targetObject = charger.gameObject;
        SetStatus(DroneStatus.GoingToCharge);
    }

    public void Collect(Collectable collectablePlace) {

        if(collectablePlace == null) {
            Debug.LogWarning("Collectable is null. Drone cant collect material.");
            return;
        }
        
        holdingMaterial = collectablePlace.Collect();
        holdingMaterial.SetCollectableActivity(false);
        holdingMaterial.transform.parent = materialPlaceholder.transform;
        holdingMaterial.MoveTo(Vector3.zero);
        holdingMaterial.StartRotation();
        SetStatus(DroneStatus.HoldingMaterial);
    }

    public void Drop(Dropable dropablePlace) {

        if(dropablePlace == null) {
            Debug.LogWarning("Dropable is null. Drone cant drop holding material.");
            return;
        }
        
        if(dropablePlace.Drop(holdingMaterial)) {

            holdingMaterial.StopRotation();
            holdingMaterial = null;
            SetStatus(DroneStatus.Free);
        }
    }

    public void Drop() {

        holdingMaterial.transform.parent = transform.parent;
        Vector3 materialTargetPos = new Vector3(transform.position.x, 0.32f, transform.position.z);
        holdingMaterial.MoveTo(materialTargetPos);

        holdingMaterial.SetCollectableActivity(true);
        holdingMaterial.StopRotation();
        holdingMaterial = null;
        SetStatus(DroneStatus.Free);
    }

    public void GoCollect(Collectable collectablePlace) {

        if(collectablePlace == null) {
            Debug.LogWarning("Collectable is null. Drone cant go to collect material.");
            return;
        }

        targetObject = collectablePlace.gameObject;
        SetStatus(DroneStatus.GoingToCollect);
    }

    public void GoDrop(Dropable dropablePlace) {

        if(dropablePlace == null) {
            Debug.LogWarning("Dropable is null. Drone cant go to droping place.");
            return;
        }

        targetObject = dropablePlace.gameObject;
        SetStatus(DroneStatus.GoingToDrop);
    }

    public void Go(Vector3 position) {

        targetPosition = position;
        SetStatus(DroneStatus.Going);
    }

    public void StopFollowingObject(GameObject obj) {

        if(obj.GetInstanceID() != targetObject.GetInstanceID())
            return;

        targetObject = null;

        switch(status) {

            case DroneStatus.GoingToCollect:
                SetStatus(DroneStatus.Free);
                break;

            case DroneStatus.GoingToDrop:
                SetStatus(DroneStatus.HoldingMaterial);
                break;
        }
    }
}
