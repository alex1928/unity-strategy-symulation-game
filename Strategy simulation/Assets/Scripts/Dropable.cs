using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Storage))]
public class Dropable : StorageInteraction
{

    public bool IsDropRequired {

        get {

            return storage.desiredAmount > storage.amount;
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    

    public bool Drop(Material mat) {

        return storage.Put(mat);
    } 
}
