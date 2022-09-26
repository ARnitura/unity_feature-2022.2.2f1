using System;
using System.Collections;
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
    private Camera cam;
    [SerializeField]
    private ARObjectLoader objectLoader;


    [Header("Model settings")]
    //later we need more data about model rotation axis
    [SerializeField] private Vector3 modelRotationAxis = Vector3.down;
    [SerializeField] private float yUpLength = 1;


    [Header("Debug")]
    //[SerializeField] private Transform debugRaycastTransform;
    [SerializeField] private TextMeshProUGUI debugText;

    private ARPlaneManager aRPlaneManager;
    private ARRaycastManager m_RaycastManager;

    //private bool objectPlaced = false;


    private Vector2 touchLockPos;
    private Vector3 currentObjectVelocity;
    private Vector3 currentModelVelocity;
    private float placedTransformPlaneY;
    private Transform placedTransform = null;
    private Transform modelTransform = null;

    public ARObject Object { get; set; }

    private bool objectPlaced = false;
    private bool isRotating = false;
    private bool touchLock = false;
    private ScanManager scanManager;

    public static Action OnRotationStart, OnRotationEnd, OnMoveStart, OnMoveEnd;

    private void Awake()
    {
        cam = Camera.main;

        m_RaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();

        scanManager = FindObjectOfType<ScanManager>();

        OnRotationEnd += OnRotationEndEvent;
        OnMoveEnd += OnMoveEndEvent;

        OnRotationStart += OnRotationStartEvent;
        OnMoveStart += OnMoveStartEvent;
    }



    private void Update()
    {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaceObject(new Vector3(2, 0, 2), Quaternion.identity);
        }

        string spawnedObjectColor = objectLoader.ARObject != null ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
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
        if (UIUtils.IsPointerOverUIObject())
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

#endif

        if (!scanManager.ScanComplete)
            return;


        if (Object == null)
            return;


        foreach (Touch touch in Input.touches)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;


        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;



        if (!objectPlaced)
        {
            List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();
            if (m_RaycastManager.Raycast(touchPosition, hitInfo, TrackableType.PlaneWithinPolygon))
            {
                PlaceObject(hitInfo[0].pose.position, hitInfo[0].pose.rotation);
                DisableVisual();
                //EnableVisual();
            }
            // PlaceObject(planeMarker.transform.position, Quaternion.identity);
        }
        else
        {
            if (Input.touchCount == 2)
                RotateObject();
            else if (Input.touchCount == 1 && !isRotating)
                MoveObject();
        }
    }

    public void AssignObject(ARObject arObject)
    {
        Object = arObject;
        modelTransform = Object.Model;
        placedTransform = Object.transform;
    }

    private void PlaceObject(Vector3 pos, Quaternion rot)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log($"Model transform = {objectLoader.LoadedModelTransform.gameObject.name}");
        //  Debug.Log($"Placed transform = {objectLoader.LoadedModelTransform.parent.gameObject.name}");
#endif


        objectPlaced = true;
        aRPlaneManager.enabled = false;
        foreach (ARPlane plane in aRPlaneManager.trackables)
            plane.gameObject.SetActive(false);

        //Vector3 hitPos = hit.pose.position;
        // Quaternion hitRot = hit.pose.rotation;

        placedTransform.gameObject.SetActive(true);
        modelTransform.gameObject.SetActive(true);

        placedTransform.position = pos;
        placedTransformPlaneY = placedTransform.position.y;
        placedTransform.rotation = rot;


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
                Ray ray = cam.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    // debugRaycastTransform.position = hitInfo.point;
#endif
                    if (hitInfo.collider.transform == modelTransform)
                    {
                        touchLock = true;
                        touchLockPos = touch.position;

                        OnMoveStart?.Invoke();
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
            Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, (placedTransform.position - cam.transform.position).magnitude);
            Vector3 projectedScreen = cam.ScreenToWorldPoint(screenPosition);
            Vector3 projectedPosition = new Vector3(projectedScreen.x, placedTransform.position.y, projectedScreen.z);
            Vector3 clampedPosition = Vector3.ClampMagnitude(projectedPosition, 5);
            clampedPosition.y = placedTransformPlaneY;

            placedTransform.position = Vector3.SmoothDamp(placedTransform.position, clampedPosition, ref currentObjectVelocity, 0.05f);
            modelTransform.localPosition = Vector3.SmoothDamp(modelTransform.localPosition, Vector3.up * yUpLength, ref currentModelVelocity, 0.05f);


        }

        //uncheck touchLock
        if (touch.phase == TouchPhase.Ended)
        {
            if (touchLock)
                OnMoveEnd?.Invoke();

            touchLock = false;
        }


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

        Vector3 targetPos = placedTransform.position;
        targetPos.y = placedTransformPlaneY;

        modelTransform.localPosition = Vector3.SmoothDamp(modelTransform.localPosition, Vector3.up * yUpLength, ref currentModelVelocity, 0.05f);
        placedTransform.position = Vector3.SmoothDamp(placedTransform.position, targetPos, ref currentObjectVelocity, 0.05f);

        //block set position while rotated
        if (!isRotating)
        {
            isRotating = true;
            OnRotationStart?.Invoke();
        }
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            touchPosition = Input.mousePosition;
            return true;
        }

        if (isRotating)
            OnRotationEnd?.Invoke();

        isRotating = false;



        touchPosition = default;
        return false;
    }
    public void ResetObject()
    {
        Object = null;
        objectPlaced = false;

        EnableVisual();
    }
    private void DisableVisual() => planeMarker.SetActive(false);
    private void EnableVisual() => planeMarker.SetActive(true);


    private void OnMoveStartEvent() => StopAllCoroutines();//Debug.Log("OnMoveStart");

    private void OnRotationStartEvent() => StopAllCoroutines();// Debug.Log("OnRotationStart");

    private void OnMoveEndEvent() => StartCoroutine(TranslateToPlane());// Debug.Log("OnMoveEnd");

    private void OnRotationEndEvent() => StartCoroutine(TranslateToPlane());// Debug.Log("OnRotationEnd");

    private IEnumerator TranslateToPlane()
    {
        float duration = 0.5f;
        float t = 0;

        Vector3 startPos = modelTransform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            modelTransform.localPosition = new Vector3(0, Mathf.SmoothStep(startPos.y, 0, t), 0);

            yield return null;
        }
    }
}