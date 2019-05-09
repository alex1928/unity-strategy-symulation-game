using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tower : MonoBehaviour
{

    public float towerRange = 20f;    

    public List<Collectable> collectables = new List<Collectable>();
    public List<Dropable> dropables = new List<Dropable>();
    public List<Charger> chargers = new List<Charger>();

    DroneManager droneManager;


    void Start()
    {
        droneManager = GetComponent<DroneManager>();

        StartCoroutine("UpdateObjectsInRange");
    }

    IEnumerator UpdateObjectsInRange() {

        while(true) {

            collectables.Clear();
            dropables.Clear();
            chargers.Clear();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, towerRange);
            foreach(Collider collider in hitColliders) {
                
                Collectable collectable = collider.gameObject.GetComponent<Collectable>();
                if(collectable != null && collectable.enabled) {
                    AddStorageInteraction(collectable);
                }

                Dropable dropable = collider.gameObject.GetComponent<Dropable>();
                if(dropable != null) {
                    AddStorageInteraction(dropable);
                }

                Charger charger = collider.gameObject.GetComponent<Charger>();
                if(charger != null) {
                    chargers.Add(charger); 
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }


     public List<StorageInteraction> GetAllStorageInteractable(MaterialType type) {
        
        List<StorageInteraction> interactions = new List<StorageInteraction>();

        interactions.AddRange(collectables);
        interactions.AddRange(dropables);

        if(type != MaterialType.All)
            interactions = interactions.FindAll(t => t.Type == type).ToList<StorageInteraction>();

        interactions = interactions.OrderByDescending(t => t.prioryty).ToList<StorageInteraction>();

        return interactions;
     }


    public List<Collectable> GetCollectablesByType(MaterialType type) {

        List<Collectable> res = new List<Collectable>();

        foreach(Collectable collectable in collectables) {

            if((type == MaterialType.All || collectable.Type == type) && !collectable.IsEmpty && collectable.IsCollectionRequired) {
                res.Add(collectable);
            }
        }

        res = res.OrderByDescending(t => t.prioryty).ToList<Collectable>();

        //res = res.OrderByDescending(t => t.prioryty).ThenByDescending(t => t.IsCollectionRequired).ToList<Collectable>();

        return res;
    }



    public Charger GetNearestCharger(Vector3 position) {

        float min = float.PositiveInfinity;
        Charger res = null;

        foreach(Charger charger in chargers) {

            float distance = Vector3.Distance(position, charger.gameObject.transform.position);

            if(distance < min) {

                res = charger;
                min = distance;
            }
        }
        
        return res;
    }


    public List<Dropable> GetDropablesByType(MaterialType type) {

        List<Dropable> res = new List<Dropable>();

        foreach(Dropable dropable in dropables) {

            if((type == MaterialType.All || dropable.Type == type) && !dropable.IsFull) {
                res.Add(dropable);
            }
        }

        res = res.OrderByDescending(t => t.prioryty).ThenByDescending(t => t.IsDropRequired).ToList<Dropable>();

        return res;
    }


    public Dropable GetNearestDropableOfType(Vector3 position, MaterialType type) {

        List<Dropable> dropablesOfType = GetDropablesByType(type);
        float min = float.PositiveInfinity;
        Dropable res = null;

        foreach(Dropable dropable in dropablesOfType) {

            float distance = Vector3.Distance(position, dropable.gameObject.transform.position);

            if(distance < min) {

                res = dropable;
                min = distance;
            }
        }
        
        return res;
    }

    
    public void AddStorageInteraction(StorageInteraction interaction) {

        interaction.assignedTower = this;

        if(interaction is Collectable) {
            collectables.Add((Collectable)interaction);
        }

        if(interaction is Dropable) {
            dropables.Add((Dropable)interaction);
        }
    }

    public void RemoveStorageInteraction(StorageInteraction interaction) {

        interaction.assignedTower = null;

        if(interaction is Collectable) {
            collectables.Remove((Collectable)interaction);
        }

        if(interaction is Dropable) {
            dropables.Remove((Dropable)interaction);
        }

        droneManager.StopDronesWithTargetObject(interaction.gameObject);
    }


    void OnDrawGizmosSelected()
    {

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position , transform.up, towerRange);
    }
}
