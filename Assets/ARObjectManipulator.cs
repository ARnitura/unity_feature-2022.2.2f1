using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectManipulator : MonoBehaviour
{
    [Header("Model settings")]
    //later we need more data about model rotation axis
    [SerializeField] private Vector3 modelRotationAxis = Vector3.down;
    [SerializeField] private float yUpLength = 1;


    private Transform placedTransform = null;
    private Transform modelTransform = null;
    public ARObject Object { get; set; }
    private bool isRotating = false;

    private ARPlaneManager aRPlaneManager;
    private ARRaycastManager m_RaycastManager;

    private Vector2 touchLockPos;
    private Vector3 currentObjectVelocity;
    private Vector3 currentModelVelocity;
    private float placedTransformPlaneY;

    private Camera cam;

    public static Action OnRotationStart, OnRotationEnd, OnMoveStart, OnMoveEnd;

    // Start is called before the first frame update
    private void Awake()
    {

        GlobalState.StateChanged += GlobalState_StateChanged;

        cam = Camera.main;

        m_RaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();

        OnRotationEnd += OnRotationEndEvent;
        OnMoveEnd += OnMoveEndEvent;

        OnRotationStart += OnRotationStartEvent;
        OnMoveStart += OnMoveStartEvent;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        enabled = obj == GlobalState.State.ARObject;
    }

    public void SelectObject(ARObject arObject)
    {
        Object = arObject;
        modelTransform = Object.Model;
        placedTransform = Object.transform;
        placedTransformPlaneY = placedTransform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Object == null)
            return;

        if (UIUtils.IsTouchOverUIObject())
            return;

        if (!TryGetTouchPosition(out Vector2 pos))
            return;


        if (Input.touchCount == 2)
            RotateObject();
        else if (Input.touchCount == 1 && !isRotating)
            MoveObject();

#if UNITY_EDITOR
        if (LeanTouch.Fingers.Count == 2)
            RotateObject();
        else if (LeanTouch.Fingers.Count == 1 && !isRotating)
            MoveObject();
#endif
        // PlaceObject(planeMarker.transform.position, Quaternion.identity);
    }


    private void MoveObject()
    {
#if !UNITY_EDITOR
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnMoveStart?.Invoke();
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            OnMoveEnd?.Invoke();
            return;
        }
#else

        if (LeanTouch.Fingers[0].Down)
        {
            OnMoveStart?.Invoke();
            //Debug.Log("starting move action");
        }
        if (LeanTouch.Fingers[0].Up)
        {
            OnMoveEnd?.Invoke();
            //Debug.Log("Ending move action");
            return;
        }
#endif
        // }
        modelTransform.localPosition = Vector3.SmoothDamp(modelTransform.localPosition, Vector3.up * yUpLength, ref currentModelVelocity, 0.05f);

        //Debug.Log($"delta is {LeanTouch.Fingers[0].ScaledDelta}");

        //WTF
        /*
        if (LeanTouch.Fingers[0].ScaledDelta.sqrMagnitude < 2f)
        {
            //Debug.Log($"blocked movement delta is {LeanTouch.Fingers[0].ScaledDelta}");
            return;
        }
        */




        /*Raycast movement
        //Debug.Log("Move model");

        //get object and transform position
        m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.Planes);
        selectedObject = placedTransform.gameObject;
        selectedObject.transform.position = s_Hits[0].pose.position;
        */

        //Vector3 worldDelta = LeanTouch.Fingers[0].GetWorldDelta(Vector3.Distance(placedTransform.position, cam.transform.position), cam);

        // Vector3 horizontalComponent = cam.transform.forward;
        //horizontalComponent.y = 0;


        //worldDelta = Vector3.ProjectOnPlane(worldDelta, horizontalComponent);
        //worldDelta = transform.InverseTransformDirection(worldDelta);

#if !UNITY_EDITOR
        Vector3 screenPos = Input.GetTouch(0).position;
        Vector3 screenDelta = Input.GetTouch(0).deltaPosition;

        float distance = Vector3.Distance(placedTransform.position, cam.transform.position);
        Vector3 worldPos1 = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
        Vector3 worldPos2 = cam.ScreenToWorldPoint(new Vector3(screenPos.x - screenDelta.x, screenPos.y - screenDelta.y, distance));
        Vector3 worldDelta = worldPos1 - worldPos2;
#else
        Vector3 worldDelta = LeanTouch.Fingers[0].GetWorldDelta(Vector3.Distance(placedTransform.position, cam.transform.position), cam);
#endif

        worldDelta = transform.InverseTransformDirection(worldDelta);
        Vector3 projectedCameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Vector3 rightDirection = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up);

        //Debug.DrawRay(placedTransform.position, rightDirection, Color.red);
        //Debug.DrawRay(placedTransform.position, projectedCameraForward, Color.blue);



        //placedTransform.position += projectedCameraForward * deltaMove.y * 0.001f + rightDirection * deltaMove.x * 0.001f;
        Vector3 desiredPosition = placedTransform.position + rightDirection * worldDelta.x + projectedCameraForward * worldDelta.y;// projectedCameraForward * touch.deltaPosition.y * 0.005f + rightDirection * touch.deltaPosition.x * 0.005f;
        Vector3 clampedPosition = Vector3.ClampMagnitude(desiredPosition, 15);
        clampedPosition.y = placedTransformPlaneY;


        placedTransform.position = clampedPosition;
        /*


        //cool movement?
        Vector3 screenPosition = new Vector3(touch.position.x, touch.position.y, (placedTransform.position - cam.transform.position).magnitude);
        Vector3 projectedScreen = cam.ScreenToWorldPoint(screenPosition);
        Vector3 projectedPosition = new Vector3(projectedScreen.x, placedTransform.position.y, projectedScreen.z);
        Vector3 clampedPosition = Vector3.ClampMagnitude(projectedPosition, 5);
        clampedPosition.y = placedTransformPlaneY;

        placedTransform.position = Vector3.SmoothDamp(placedTransform.position, clampedPosition, ref currentObjectVelocity, 0.05f);
        modelTransform.localPosition = Vector3.SmoothDamp(modelTransform.localPosition, Vector3.up * yUpLength, ref currentModelVelocity, 0.05f);
        */





    }


    private void RotateObject()
    {
        //get finger and angle
        //List<Lean.Touch.LeanFinger> finger = Lean.Touch.LeanTouch.Fingers;
#if !UNITY_EDITOR
        Touch touch0 = Input.touches[0];
        Touch touch1 = Input.touches[1];

        var pos1 = touch0.position;
        var pos2 = touch1.position;
        var pos1b = touch0.position - touch0.deltaPosition;
        var pos2b = touch1.position - touch1.deltaPosition;

        var screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        float twistDegrees = Vector3.SignedAngle(pos2b - pos1b, pos2 - pos1, Vector3.forward);
        //Debug.Log(twistDegrees);
#else
        float twistDegrees = LeanGesture.GetTwistDegrees();
#endif
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

    private void OnMoveStartEvent()
    {
        StopAllCoroutines();//Debug.Log("OnMoveStart");
    }

    private void OnRotationStartEvent()
    {
        StopAllCoroutines();// Debug.Log("OnRotationStart");
    }

    private void OnMoveEndEvent()
    {
        StartCoroutine(TranslateToPlane());// Debug.Log("OnMoveEnd");
    }

    private void OnRotationEndEvent()
    {
        StartCoroutine(TranslateToPlane());// Debug.Log("OnRotationEnd");
    }

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

    //todo - remove and merge with Update logic
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
        if (isRotating)
            OnRotationEnd?.Invoke();

        isRotating = false;
        touchPosition = default;
        return false;
    }
}
