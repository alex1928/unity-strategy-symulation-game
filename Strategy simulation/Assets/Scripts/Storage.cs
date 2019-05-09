using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    
    public MaterialType storageType = MaterialType.ConcreteOre;
    public GameObject materialPrefab;
    public int amount = 0;
    public int maxAmount = 0;

    public int desiredAmount = 1;

    public bool showMaterials = false;
    
    public List<Transform> materialsPlaceholders = new List<Transform>();
    public List<Material> materialsInStorage = new List<Material>();


    void Start() {

        if(showMaterials) {

            maxAmount = materialsPlaceholders.Count;

            for(int i = 0; i < amount; i++) {

                GameObject matObject = Instantiate(materialPrefab);
                Material mat = matObject.GetComponent<Material>();
                Transform placeholder = materialsPlaceholders[i];
                materialsInStorage.Add(mat);
                matObject.transform.position = placeholder.position;
                matObject.transform.rotation = placeholder.rotation;
            }
        }
        else if(maxAmount == 0 || amount > maxAmount) {

            maxAmount = amount;
        }
    }


    public Material Get() {

        if(amount <= 0) {
            return null;
        }
        
        amount--;

        if(showMaterials) {
            
            Material resMat = materialsInStorage[amount];
            materialsInStorage.Remove(resMat);
            return resMat;
        }
        else
        {
            return Instantiate(materialPrefab).GetComponent<Material>();
        }
    }


    public bool Put(Material material) {
        
        if(storageType != material.type) {
            return false;
        }

        if(amount >=  maxAmount) {
            return false;
        }   

        if(showMaterials) {
            
            Transform placeholder = GetEmptyPlaceholder();
            material.transform.parent = placeholder.transform;
            material.MoveTo(Vector3.zero);
            material.transform.rotation = placeholder.rotation;
            materialsInStorage.Add(material);
        }
        else
        {
            Destroy(material.gameObject);
        }

        amount++;

        return true;
    }


    private Transform GetEmptyPlaceholder() {

        return materialsPlaceholders[amount];
    }
}
