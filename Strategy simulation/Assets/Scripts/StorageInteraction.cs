using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StorageInteraction : MonoBehaviour
{

    protected Storage storage;
    public int prioryty = 0;

    public float range;

    [HideInInspector] public Tower assignedTower;

    public MaterialType Type {

        get {
            return storage.storageType;
        }
    }

    public bool IsFull {

        get {

            return storage.amount >= storage.maxAmount;
        }
    }

    public bool IsEmpty {

        get {

            return storage.amount <= 0;
        }
    }

    public void Awake() {

        storage = GetComponent<Storage>();
    }


    void OnDrawGizmosSelected()
    {

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, range);
    }

    public bool IsRelatedWith(StorageInteraction interaction) {

        if(storage == null || interaction == null)
            return false;

        return storage.GetInstanceID() == interaction.storage.GetInstanceID();
    }

}
