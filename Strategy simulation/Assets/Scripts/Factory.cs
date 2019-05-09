using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{

    public Storage inStorage;
    public Storage outStorage;

    public MaterialType resource;
    public Material product;

    public float conversionTime = 5f;

    public float currentTime = 0f;
    public bool isConverting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isConverting) {

            if(currentTime > 0)
                currentTime -= Time.deltaTime;

            if(currentTime <= 0)
                FinishProducting();
        }
        else
        {
            if(HasIngridients() && HasSpace())
                StartProducting();
        }
    }

    bool HasIngridients() {

        return inStorage.amount > 0;
    }

    bool HasSpace() {

        return outStorage.amount < outStorage.maxAmount;
    }

    void StartProducting() {
        
        Material material = inStorage.Get();
        Destroy(material.gameObject);

        currentTime = conversionTime;
        isConverting = true;
    }

    void FinishProducting() {

        if(outStorage.amount < outStorage.maxAmount) {

            Material material = Instantiate(product.gameObject).GetComponent<Material>();
            material.SetCollectableActivity(false);
            if(outStorage.Put(material))
                isConverting = false;
        }
    }

    public float GetProgress() {

        return (conversionTime - currentTime) / conversionTime;
    }
}
