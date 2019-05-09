using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[RequireComponent(typeof(Tower))]
public class DroneManager : MonoBehaviour
{

    Tower tower;
    public int dronesInside = 5;
    public int dronesToRelease = 5;
    public GameObject dronePrefab;
    public Transform droneSpawnPoint;
    public Transform droneFirstTarget;
    public List<Drone> drones = new List<Drone>();

    public bool lastDroneActionWasCollectable = false;


    public delegate void StopFollowingObject(GameObject obj);
    public static event StopFollowingObject OnObjectDelete;

    
    void Start() {

        tower = GetComponent<Tower>();

        StartCoroutine("SpawnDrones");   
    }


    void Update()
    {
        
        foreach(Drone drone in drones) {

            switch(drone.status) {

                case DroneStatus.Free:
                    
                    if(Charge(drone)) break;
                    if(CollectMaterial(drone)) break;

                    break;

                case DroneStatus.GoingToCollect:
                    if(Charge(drone)) break;
                    break;

                case DroneStatus.HoldingMaterial:

                    List<Dropable> dropables = tower.GetDropablesByType(drone.holdingMaterial.type);

                    if(dropables.Count == 0)
                        break;

                    if(drone.targetObject == null)
                        drone.GoDrop(dropables[0]);

                    Collectable collectable = drone.targetObject.GetComponent<Collectable>();

                    if(collectable == null)
                        drone.GoDrop(dropables[0]);

                    foreach(Dropable dropable in dropables) {
                        
                        if(!dropable.IsRelatedWith(collectable)) {
                            drone.GoDrop(dropable);
                        }
                    }
                    break;
            }
        }
    }

    
    public bool Charge(Drone drone) {

        if(!drone.NeedsCharge)
            return false;

        Charger charger = tower.GetNearestCharger(drone.transform.position);

        drone.Charge(charger);
        return true;
    }

    public bool CollectMaterial(Drone drone) {

        List<StorageInteraction> interactions = tower.GetAllStorageInteractable(MaterialType.All);

        foreach(StorageInteraction interacion in interactions) {

            Collectable materialToCollect = null;

            if(interacion is Collectable && !lastDroneActionWasCollectable) {

                Collectable collectable = (Collectable)interacion;

                List<Dropable> dropables = tower.GetDropablesByType(collectable.Type);
                if(dropables.Count > 0) {
                    foreach(Dropable dropable in dropables) {

                        if(!collectable.IsRelatedWith(dropable)) {
                            materialToCollect = collectable;
                            break;
                        }
                    }
                }
            }

            if(interacion is Dropable && lastDroneActionWasCollectable) {

                Dropable dropable = (Dropable)interacion;

                List<Collectable> collectables = tower.GetCollectablesByType(dropable.Type);

                if(collectables.Count > 0) {
                    foreach(Collectable collectable in collectables) {

                        if(!collectable.IsRelatedWith(dropable)) {
                            materialToCollect = collectable;
                            break;
                        }
                    }
                    
                }
            }
            
            if(materialToCollect != null) {

                if(materialToCollect.ReserveForDrone()) {
                    
                    drone.GoCollect(materialToCollect);
                    lastDroneActionWasCollectable = !lastDroneActionWasCollectable;
                    return true;
                }
                materialToCollect = null;
            }
        }

        lastDroneActionWasCollectable = !lastDroneActionWasCollectable;

        return false;
    }

    


    public void ReleaseDrones(int amount) {

        if(amount > dronesInside) 
            amount = dronesInside;

        dronesToRelease = amount;
    }


    public void StopDronesWithTargetObject(GameObject obj) {

        OnObjectDelete(obj);
    }


    IEnumerator SpawnDrones() {
        
        while(true) {
            
            if(dronesToRelease > 0 && dronesInside > 0) {

                dronesToRelease--;
                dronesInside--;
                GameObject droneObject = Instantiate(dronePrefab);
                droneObject.transform.position = droneSpawnPoint.position;
                droneObject.transform.rotation = droneSpawnPoint.rotation;

                Drone drone = droneObject.GetComponent<Drone>();

                drones.Add(drone);
                
                drone.Go(droneFirstTarget.position);
            }

            yield return new WaitForSeconds(2);
        }
    }
}
