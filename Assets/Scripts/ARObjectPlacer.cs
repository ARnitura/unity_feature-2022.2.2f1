using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using TriLibCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARObjectPlacer : MonoBehaviour
{
    //marker
    [SerializeField] GameObject visualObject;

    [SerializeField] Camera ARCamera;

    [SerializeField] ARObjectLoader objectLoader;

    //later we need more data about model rotation axis
    [field:SerializeField] public Vector3 RotationAxis { get; set; } = Vector3.down;

    private ARPlaneManager aRPlaneManager;

    [SerializeField]
    Transform debugRaycastTransform;
    

    private GameObject selectedObject;

    


    List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARRaycastManager m_RaycastManager;

    Action onObjectReplace;

    bool objectPlaced = false;
    bool isRotating = false;
    bool touchLock = false;

   // [SerializeField]
    //bool debugMode = false;

    public void ResetObject()
    {
        objectPlaced = false;
    }

    void Awake()
    {
        // loadModel(testPath);
        
        m_RaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();


        onObjectReplace += DisableVisual;
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        isRotating = false;

        touchPosition = default;
        return false;
    }

    [SerializeField]
    TextMeshProUGUI debugText;

    void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        string spawnedObjectColor = objectLoader.SpawnedObject != null ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        string touchColor = ColorUtility.ToHtmlStringRGB(Color.green);
        string touchAppliedColor = TryGetTouchPosition(out Vector2 touchPosition1)?  ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        bool touchUIBlock = false;

        foreach (Touch touch in Input.touches)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                touchColor = ColorUtility.ToHtmlStringRGB(Color.red);
                touchUIBlock = true;
                break;
            }
        bool verdict = !touchUIBlock && objectLoader.SpawnedObject != null && TryGetTouchPosition(out touchPosition1);
        string verdictColor = verdict? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);

        debugText.text = $"Touch conditions:\n" +
            $"Model loaded? <color=#{spawnedObjectColor}>{objectLoader.SpawnedObject != null}</color>\n" +
            $"Pointer(s) is over UI? <color=#{touchColor}>{touchUIBlock}</color>\n" +
            $"At least one touch? <color=#{touchAppliedColor}>{TryGetTouchPosition(out touchPosition1)}</color>\n" +
            $"Can place 3d model? <color=#{verdictColor}>{verdict}</color>";

#endif
        if (objectLoader.SpawnedObject == null)
            return;

        foreach (Touch touch in Input.touches)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;


        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;



        if (!objectPlaced)
        {
            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
                PlaceObject();
        }
        else
        {
            ProcessMovement();
        }
        


    }

    void PlaceObject()
    {
        if (!objectPlaced)
            DisableVisual();

        var hitPos = s_Hits[0].pose.position;
        var hitRot = s_Hits[0].pose.rotation;




        aRPlaneManager.enabled = false;

        foreach (var plane in aRPlaneManager.trackables)
            plane.gameObject.SetActive(false);

        objectLoader.SpawnedObject.gameObject.SetActive(true);
        objectLoader.SpawnedObject.position = hitPos;
        objectLoader.SpawnedObject.rotation = hitRot;
        //objectLoader.spawnedObject.gameObject.tag = "UnSelected";

        objectPlaced = true;
    }


    Vector2 touchLockPos;
    Vector3 currentVelocity;
    float touchLockY;
    void ProcessMovement()
    {

        // rotate object
        if (Input.touchCount == 2)
        {
            //Debug.Log("Rotation move");

            //get finger and angle
            var finger = Lean.Touch.LeanTouch.Fingers;
            var twistDegrees = Lean.Touch.LeanGesture.GetTwistDegrees(finger) * 1;
            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            //rotate object
            //m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.Planes);
            selectedObject = objectLoader.SpawnedObject.gameObject;
            selectedObject.transform.Rotate(RotationAxis, twistDegrees);
            selectedObject.transform.Rotate(RotationAxis, twistDegrees);

            //block set position while rotated
            isRotating = true;
        }

        if (Input.touchCount == 1 && !isRotating)
        {
            //get touch count and position
            Touch touch = Input.GetTouch(0);
            var touchPosition = touch.position;

            if (!touchLock)
            {
                //process touch lock
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = ARCamera.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(ray, out RaycastHit hitInfo))
                    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                        debugRaycastTransform.position = hitInfo.point;
                        //debugRaycastTransform.forward = hitInfo.normal;  
#endif

                        if (hitInfo.collider.transform == objectLoader.SpawnedObject)
                        {
                            touchLock = true;
                            touchLockPos = touch.position;
                            touchLockY = objectLoader.SpawnedObject.position.y;
                        }
                    }
                }
            }


            if (touchLock)
            {
                //move
                //if (touch.phase == TouchPhase.Moved && isRotating == false)
                if (isRotating == false)
                {
                    /*Raycast movement
                    //Debug.Log("Move model");

                    //get object and transform position
                    m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.Planes);
                    selectedObject = objectLoader.SpawnedObject.gameObject;
                    selectedObject.transform.position = s_Hits[0].pose.position;
                    */

                    /* Simple movement
                    Vector2 deltaMove = touch.position - touchLockPos;
                    Vector3 projectedCameraForward = Vector3.ProjectOnPlane(ARCamera.transform.forward, Vector3.up);
                    Vector3 rightDirection = -Vector3.Cross(projectedCameraForward, Vector3.up);
                    objectLoader.SpawnedObject.position += projectedCameraForward * touch.deltaPosition.y * 0.001f + rightDirection * touch.deltaPosition.x * 0.001f;
                    */

                    //cool movement?
                    Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, (objectLoader.SpawnedObject.position - ARCamera.transform.position).magnitude);
                    Vector3 projectedScreen = ARCamera.ScreenToWorldPoint(screenPosition);
                    Vector3 projectedPosition = new Vector3(projectedScreen.x, objectLoader.SpawnedObject.position.y, projectedScreen.z);
                    Vector3 clampedPosition = Vector3.ClampMagnitude(projectedPosition, 5);
                    clampedPosition.y = touchLockY;

                    objectLoader.SpawnedObject.position = Vector3.SmoothDamp(objectLoader.SpawnedObject.position, clampedPosition, ref currentVelocity, 0.05f);




                }


            }
            //set tag "UnSelected"
            if (touch.phase == TouchPhase.Ended)
            {
                touchLock = false;
            }
        }
    }

    public void DisableVisual()
    {
        visualObject.SetActive(false);
        //Debug.Log("Visual disabled");
    }

    public void EnableVisual()
    {
        visualObject.SetActive(true);
    }
}