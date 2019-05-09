using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Drone))]
public class DroneStatusBadge : MonoBehaviour
{

    public GameObject badgePrefab;
    private Drone drone;
    private Camera cam;

    public float YOffset = 3;
    private GameObject badge;
    private Text badgeText;

    // Start is called before the first frame update
    void Start()
    {
        
        cam = Camera.main;
        drone = GetComponent<Drone>();

        badge = Instantiate(badgePrefab);
        badge.transform.SetParent(UIManager.instance.canvas.transform, false);
        
        badgeText = badge.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 dronePos = drone.transform.position;
        dronePos.y += YOffset;

        Vector3 badgePosition = cam.WorldToScreenPoint(dronePos);
        badge.transform.position = badgePosition;

        string text = GetStatus();
        badgeText.text = text;
    }


    string GetStatus() {

        switch(drone.status) {
            
            case DroneStatus.Free:
                return "Wolny";

            case DroneStatus.Charging:
                return "Ładuję baterie";
            
            case DroneStatus.GoingToCollect:
                return "Idę po zasoby";

            case DroneStatus.GoingToDrop:
                return "Odkładam zasoby";

            case DroneStatus.Collecting:
                return "Zbieram zasoby";
            
            case DroneStatus.GoingToCharge:
                return "Idę naładować baterie";

            case DroneStatus.HoldingMaterial:
                return "Trzymam zasoby";

            case DroneStatus.Going:
                return "Idę do celu";
        }

        return "";
    }
}
