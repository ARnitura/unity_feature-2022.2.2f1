using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARObjectPlacer : MonoBehaviour
{
    //marker
    [SerializeField] private GameObject planeMarker;
    [SerializeField] private Camera ARCamera;
    [SerializeField] private ARObjectLoader objectLoader;


    [Header("Model settings")]
    //later we need more data about model rotation axis
    [SerializeField] private Vector3 modelRotationAxis = Vector3.down;


    [Header("Debug")]
    [SerializeField] private Transform debugRaycastTransform;
    [SerializeField] private TextMeshProUGUI debugText;

    private ARPlaneManager aRPlaneManager;
    private ARRaycastManager m_RaycastManager;

    //private bool objectPlaced = false;
    private bool isRotating = false;
    private bool touchLock = false;

    private Vector2 touchLockPos;
    private Vector3 currentVelocity;
    private float touchLockY;
    private Transform placedTransform = null;

    private void Awake()
    {
        // loadModel(testPath);

        m_RaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }
    private void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        string spawnedObjectColor = objectLoader.LoadedModelTransform != null ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        string touchColor = ColorUtility.ToHtmlStringRGB(Color.green);
        string touchAppliedColor = TryGetTouchPosition(out Vector2 touchPosition1) ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        bool touchUIBlock = false;

        foreach (Touch touch in Input.touches)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                touchColor = ColorUtility.ToHtmlStringRGB(Color.red);
                touchUIBlock = true;
                break;
            }
        bool verdict = !touchUIBlock && objectLoader.LoadedModelTransform != null && TryGetTouchPosition(out touchPosition1);
        string verdictColor = verdict ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);

        debugText.text = $"Touch conditions:\n" +
            $"Model loaded? <color=#{spawnedObjectColor}>{objectLoader.LoadedModelTransform != null}</color>\n" +
            $"Pointer(s) is over UI? <color=#{touchColor}>{touchUIBlock}</color>\n" +
            $"At least one touch? <color=#{touchAppliedColor}>{TryGetTouchPosition(out touchPosition1)}</color>\n" +
            $"Can place 3d model? <color=#{verdictColor}>{verdict}</color>";

#endif
        if (objectLoader.LoadedModelTransform == null)
            return;

        foreach (Touch touch in Input.touches)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;


        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;



        if (placedTransform == null)
        {
            List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();
            if (m_RaycastManager.Raycast(touchPosition, hitInfo, TrackableType.PlaneWithinPolygon))
            {
                placedTransform = objectLoader.LoadedModelTransform;

                PlaceObject(hitInfo[0]);
                EnableVisual();
            }
        }
        else
        {
            if (Input.touchCount == 2)
                RotateObject();
            else if (Input.touchCount == 1 && !isRotating)
                MoveObject();
        }
    }

    private void PlaceObject(ARRaycastHit hit)
    {
        aRPlaneManager.enabled = false;
        foreach (ARPlane plane in aRPlaneManager.trackables)
            plane.gameObject.SetActive(false);

        Vector3 hitPos = hit.pose.position;
        Quaternion hitRot = hit.pose.rotation;

        placedTransform.gameObject.SetActive(true);
        placedTransform.position = hitPos;
        placedTransform.rotation = hitRot;
    }
    private void MoveObject()
    {
        //get touch count and position
        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = touch.position;

        //check if raycast hits placedTransform and set touchLock
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
#endif
                    if (hitInfo.collider.transform == placedTransform)
                    {
                        touchLock = true;
                        touchLockPos = touch.position;
                        touchLockY = placedTransform.position.y;
                    }
                }
            }
        }
        else if (isRotating == false) //if we already touched object... and not rotating it
        {
            /*Raycast movement
            //Debug.Log("Move model");

            //get object and transform position
            m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.Planes);
            selectedObject = placedTransform.gameObject;
            selectedObject.transform.position = s_Hits[0].pose.position;
            */

            /* Simple movement
            Vector2 deltaMove = touch.position - touchLockPos;
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(ARCamera.transform.forward, Vector3.up);
            Vector3 rightDirection = -Vector3.Cross(projectedCameraForward, Vector3.up);
            placedTransform.position += projectedCameraForward * touch.deltaPosition.y * 0.001f + rightDirection * touch.deltaPosition.x * 0.001f;
            */

            //cool movement?
            Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, (placedTransform.position - ARCamera.transform.position).magnitude);
            Vector3 projectedScreen = ARCamera.ScreenToWorldPoint(screenPosition);
            Vector3 projectedPosition = new Vector3(projectedScreen.x, placedTransform.position.y, projectedScreen.z);
            Vector3 clampedPosition = Vector3.ClampMagnitude(projectedPosition, 5);
            clampedPosition.y = touchLockY;

            placedTransform.position = Vector3.SmoothDamp(placedTransform.position, clampedPosition, ref currentVelocity, 0.05f);

        }

        //uncheck touchLock
        if (touch.phase == TouchPhase.Ended)
            touchLock = false;
    }
    private void RotateObject()
    {
        //get finger and angle
        List<Lean.Touch.LeanFinger> finger = Lean.Touch.LeanTouch.Fingers;
        float twistDegrees = Lean.Touch.LeanGesture.GetTwistDegrees(finger) * 1;
        Touch touch1 = Input.touches[0];
        Touch touch2 = Input.touches[1];

        //rotate object
        placedTransform.Rotate(modelRotationAxis, twistDegrees);
        placedTransform.Rotate(modelRotationAxis, twistDegrees);

        //block set position while rotated
        isRotating = true;
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
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
    public void ResetObject()
    {
        placedTransform = null;
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
}