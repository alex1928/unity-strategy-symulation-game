using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Factory))]
public class FactoryProgressBar : MonoBehaviour
{
    public GameObject progressBarPrefab;
    private Factory factory;
    private Camera cam;

    public float YOffset = 4;

    private GameObject progressBar;

    private RectTransform progressElement;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        factory = GetComponent<Factory>();

        progressBar = Instantiate(progressBarPrefab);
        progressBar.transform.SetParent(UIManager.instance.canvas.transform, false);
        progressElement = FindProgressElement(progressBar);
        
        SetProgress(0);
        progressBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 factoryPos = factory.transform.position;
        factoryPos.y += YOffset;

        Vector3 badgePosition = cam.WorldToScreenPoint(factoryPos);
        progressBar.transform.position = badgePosition;
        
        if(factory.isConverting != progressBar.activeSelf)
            progressBar.SetActive(factory.isConverting);

        if(factory.isConverting) {

            SetProgress(factory.GetProgress());
        }
    }

    void SetProgress(float progress) {

        float maxWidth = progressBar.GetComponent<RectTransform>().sizeDelta.x;
        progressElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * progress);
    }


    RectTransform FindProgressElement(GameObject progressBar) {

        RectTransform[] rectTransforms = progressBar.GetComponentsInChildren<RectTransform>();
        foreach(RectTransform rTransform in rectTransforms) {

            if(rTransform.gameObject.GetInstanceID() != progressBar.gameObject.GetInstanceID())
                return rTransform;
        }

        return null;
    }
}
