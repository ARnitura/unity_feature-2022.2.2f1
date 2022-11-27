
using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.Touch;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARObjectPlacer : MonoBehaviour
{

    private Camera cam;

    [SerializeField]
    private bool showDebugInfo = false;
    //marker
    [SerializeField]
    private GameObject planeMarker;

    [SerializeField]
    private ARObjectLoader objectLoader;




    [Header("Debug")]
    //[SerializeField] private Transform debugRaycastTransform;
    [SerializeField] private TextMeshProUGUI debugText;

    //private bool objectPlaced = false;

    private ARPlaneManager aRPlaneManager;
    private ARRaycastManager m_RaycastManager;

    /*

    */
    private Transform placedTransform = null;
    private Transform modelTransform = null;

    public ARObject Object { get; set; }



    private void Awake()
    {
        cam = Camera.main;

        m_RaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();

        //scanManager = FindObjectOfType<ScanManager>();



        GlobalState.StateChanged += GlobalState_StateChanged;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        enabled = obj == GlobalState.State.ARObjectPlacement;
    }

    private void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (showDebugInfo)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlaceObject(new Vector3(2, 0, 2), Quaternion.identity);
            }

            string spawnedObjectColor = objectLoader.ARObject != null ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
            string touchColor = ColorUtility.ToHtmlStringRGB(Color.green);
            string touchAppliedColor = TryGetTouchPosition(out Vector2 touchPosition1) ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
            bool touchUIBlock = false;

            /*
            foreach (Touch touch in Input.touches)
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    touchColor = ColorUtility.ToHtmlStringRGB(Color.red);
                    touchUIBlock = true;
                    break;
                }
            */
            if (UIUtils.IsTouchOverUIObject())
            {
                touchColor = ColorUtility.ToHtmlStringRGB(Color.red);
                touchUIBlock = true;
            }
            bool verdict = !touchUIBlock && objectLoader.ARObject != null && TryGetTouchPosition(out touchPosition1);
            string verdictColor = verdict ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);

            debugText.text = $"Touch conditions:\n" +
                $"Model loaded? <color=#{spawnedObjectColor}>{objectLoader.ARObject != null}</color>\n" +
                $"Pointer(s) is over UI? <color=#{touchColor}>{touchUIBlock}</color>\n" +
                $"At least one touch? <color=#{touchAppliedColor}>{TryGetTouchPosition(out touchPosition1)}</color>\n" +
                $"Can place 3d model? <color=#{verdictColor}>{verdict}</color>";
        }
#endif



        /*
        if (!scanManager.ScanComplete)
            return;
        */

        if (Object == null)
            return;

        if (UIUtils.IsTouchOverUIObject())
            return;


        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;


        List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();
        if (m_RaycastManager.Raycast(touchPosition, hitInfo, TrackableType.PlaneWithinPolygon))
        {
            PlaceObject(hitInfo[0].pose.position, hitInfo[0].pose.rotation);
            DisableVisual();
            //EnableVisual();
        }
#if UNITY_EDITOR
        //List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();
        if (Physics.Raycast(cam.ScreenPointToRay(LeanTouch.Fingers[0].ScreenPosition), out RaycastHit hit))
        {
            PlaceObject(hit.point, Quaternion.identity);
            DisableVisual();
            //EnableVisual();
        }
#endif




    }

    public void SelectObject(ARObject arObject)
    {
        Object = arObject;
        modelTransform = Object.Model;
        placedTransform = Object.transform;
    }

    private void PlaceObject(Vector3 pos, Quaternion rot)
    {
#if UNITY_EDITOR || true
        // Debug.Log($"Model transform = {objectLoader.LoadedModelTransform.gameObject.name}");
        //  Debug.Log($"Placed transform = {objectLoader.LoadedModelTransform.parent.gameObject.name}");
#endif


        //objectPlaced = true;
        aRPlaneManager.enabled = false;
        foreach (ARPlane plane in aRPlaneManager.trackables)
            plane.gameObject.SetActive(false);

        //Vector3 hitPos = hit.pose.position;
        // Quaternion hitRot = hit.pose.rotation;

        placedTransform.gameObject.SetActive(true);
        modelTransform.gameObject.SetActive(true);

        placedTransform.position = pos;
        //placedTransformPlaneY = placedTransform.position.y;
        placedTransform.rotation = rot;

        GetComponent<ARObjectManipulator>().SelectObject(Object);
        GlobalState.SetState(GlobalState.State.ARObject);

    }



    public void ResetObject()
    {
        StopAllCoroutines();
        Object = null;
        //objectPlaced = false;

        EnableVisual();
    }
    private void DisableVisual()
    {
        planeMarker.SetActive(false);
    }

    private void EnableVisual()
    {
        planeMarker.SetActive(true);
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {

        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#if UNITY_EDITOR
        else if (LeanTouch.Fingers.Count > 0)
        {
            touchPosition = LeanTouch.Fingers[0].ScreenPosition;
            return true;
        }
#endif
        touchPosition = default;
        return false;
    }
}