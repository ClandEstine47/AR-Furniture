using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.UI;

public class InputManager : ARBaseGestureInteractable
{
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Touch touch;
    [SerializeField]
    private GameObject crosshair;
    private Pose pose;
    public List<GameObject> furnitures = new List<GameObject>();
    [SerializeField]
    public Button deleteBtn;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if(gesture.targetObject == null)
        {
            return true;
        }
        return false;

    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if(gesture.isCanceled)
        {
            return;
        }

        if(gesture.targetObject != null || IsPointerOverUI(gesture))
        {
            return;
        }
        if (GestureTransformationUtility.Raycast(gesture.startPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            GameObject placedObj = Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);
            furnitures.Add(placedObj);
            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.transform.position = pose.position;
            anchorObject.transform.rotation = pose.rotation;
            placedObj.transform.parent = anchorObject.transform;
        }

    }

    public void DeleteFurnitures()
    {
        for(int i = 0; i<furnitures.Count; i++)
        {
            Destroy(furnitures[i]);
            if (furnitures[i])
            {
                furnitures.Remove(furnitures[i]);
            }
        }
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        CrosshairCalculation();
        /*
        touch = Input.GetTouch(0);

        if (Input.touchCount < 0 || touch.phase != TouchPhase.Began)
            return;

        if (IsPointerOverUI(touch))
            return;

        Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);
        */
    }


    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventdata = new PointerEventData(EventSystem.current);
        eventdata.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventdata, results);
        return results.Count > 0;

    }

    void CrosshairCalculation()
    {
        Vector3 origin = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        //Ray ray = arCamera.ScreenPointToRay(origin);
        if (GestureTransformationUtility.Raycast(origin, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }
}



