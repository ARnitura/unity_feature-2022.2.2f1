using FlutterUnityIntegration;
using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.ARFoundation;

public class ARAnchorEditor : MonoBehaviour
{
    [SerializeField]
    private Transform anchorEditArrows;

    private ARWallAnchor selectedAnchor;

    private enum InputState { None, Down, Pressed, Up }
    private InputState currentInputState;


    [SerializeField]
    private GameObject previewRuler;

    private void Awake()
    {
        GlobalState.StateChanged += GlobalState_StateChanged;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {

    }

    private void UpdatePreviewRuler(Vector3 from, Vector3 to)
    {
        Vector3 crossProduct = Vector3.Cross(from - to, Vector3.up).normalized;

        bool reverseAngle = Vector3.SignedAngle(Camera.main.transform.forward, from - to, Vector3.up) > 0;

        previewRuler.transform.position = from / 2 + to / 2;
        previewRuler.transform.position += reverseAngle ? -crossProduct / 5f : crossProduct / 5f;


        previewRuler.transform.rotation = reverseAngle ? Quaternion.LookRotation(from - to, Vector3.up) : Quaternion.LookRotation(to - from, Vector3.up);

        float distance = Vector3.Distance(to, from);
        float roundedDistance = Mathf.Round(distance * 10) / 10;

        Debug.DrawRay(from / 2 + to / 2, (reverseAngle ? crossProduct * -0.75f : crossProduct * 0.75f) + Vector3.up * 0.1f);
        previewRuler.GetComponentInChildren<TextMeshPro>().transform.position = from / 2 + to / 2 + (reverseAngle ? crossProduct * -0.75f : crossProduct * 0.75f) + Vector3.up * 0.1f;
        //previewRuler.GetComponentInChildren<TextMeshPro>().transform.position = hit.point - (hit.point - previousAnchor.transform.position).normalized * 0.3f + Vector3.up * 0.3f;
        previewRuler.GetComponentInChildren<TextMeshPro>().transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        //previewRuler.GetComponentInChildren<TextMeshPro>().transform.position -= Vector3.Cross(hit.point - previousAnchor.transform.position, Vector3.up).normalized * 0.3f;

        previewRuler.GetComponentInChildren<TextMeshPro>().text = $"{roundedDistance}m";
        foreach (var item in previewRuler.GetComponentsInChildren<SpriteRenderer>())
        {
            item.size = new Vector2(distance, 0.9f);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (UIUtils.IsTouchOverUIObject())
            return;

        if (TryGetTouchPosition(out Vector2 touchPosition))
        {
            if (GetFirstTouchUp())
                currentInputState = InputState.Up;
            else
            if (GetFirstTouchDown())
                currentInputState = InputState.Down;
            else
                currentInputState = InputState.Pressed;
        }
        else
            currentInputState = InputState.None;

        if (GlobalState.CurrentState == GlobalState.State.ARWallEdit)
        {
            if (selectedAnchor != null && currentInputState == InputState.Pressed)
            {
                //move selected anchor
                MoveObject();

                previewRuler.gameObject.SetActive(true);
                if (selectedAnchor.NextAnchor != null)
                    UpdatePreviewRuler(selectedAnchor.NextAnchor.transform.position, selectedAnchor.transform.position);
                else
                    UpdatePreviewRuler(selectedAnchor.PreviousAnchor.transform.position, selectedAnchor.transform.position);

                foreach (var item in selectedAnchor.ConnectedWalls)
                    item.UpdateMesh();

            }



            if (currentInputState != InputState.Down)
                return;



            //try to locate and move existing nachors
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hit))
            {
                SelectAnchor(hit.transform.GetComponent<ARWallAnchor>());
            }
        }

        if (GlobalState.CurrentState == GlobalState.State.ARObject)
        {
            if (currentInputState != InputState.Down)
                return;



            //try to locate and move existing nachors
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hit))
            {
                ARWallObject wallObject = hit.transform.GetComponent<ARWallObject>();
                if (wallObject != null)
                {
                    GlobalState.SetState(GlobalState.State.ARWallEdit);
                    SelectAnchor(null, false);
                    previewRuler.gameObject.SetActive(true);
                    UpdatePreviewRuler(wallObject.FromAnchor.transform.position, wallObject.ToAnchor.transform.position);
                }
            }
        }
    }



    private void MoveObject()
    {
#if !UNITY_EDITOR
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //OnMoveStart?.Invoke();
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
           // OnMoveEnd?.Invoke();
            return;
        }
#else


#endif


        //modelTransform.localPosition = Vector3.SmoothDamp(modelTransform.localPosition, Vector3.up * yUpLength, ref currentModelVelocity, 0.05f);

        Camera cam = Camera.main;
        float anchorY = selectedAnchor.transform.position.y;

#if !UNITY_EDITOR
        Vector3 screenPos = Input.GetTouch(0).position;
        Vector3 screenDelta = Input.GetTouch(0).deltaPosition;

        float distance = Vector3.Distance(selectedAnchor.transform.position, cam.transform.position);
        Vector3 worldPos1 = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
        Vector3 worldPos2 = cam.ScreenToWorldPoint(new Vector3(screenPos.x - screenDelta.x, screenPos.y - screenDelta.y, distance));
        Vector3 worldDelta = worldPos1 - worldPos2;
#else
        Vector3 worldDelta = LeanTouch.Fingers[0].GetWorldDelta(Vector3.Distance(selectedAnchor.transform.position, cam.transform.position), cam);
#endif

        worldDelta = transform.InverseTransformDirection(worldDelta);
        Vector3 projectedCameraForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Vector3 rightDirection = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up);

        Vector3 desiredPosition = selectedAnchor.transform.position + rightDirection * worldDelta.x + projectedCameraForward * worldDelta.y;
        Vector3 clampedPosition = Vector3.ClampMagnitude(desiredPosition, 40);
        clampedPosition.y = anchorY;


        selectedAnchor.transform.position = clampedPosition;
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

    private bool GetFirstTouchDown()
    {
#if UNITY_EDITOR
        return LeanTouch.Fingers[0].Down;
#else
        return Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    private bool GetFirstTouchUp()
    {
#if UNITY_EDITOR
        return LeanTouch.Fingers[0].Up;
#else
        return Input.GetTouch(0).phase == TouchPhase.Ended;
#endif
    }

    internal void SelectAnchor(ARWallAnchor anchor, bool enterArObjectStateIfNull = true)
    {
        var previousSelected = selectedAnchor;

        if (previousSelected != null)
            previousSelected.GetComponentInChildren<Renderer>().material.color = Color.white;

        selectedAnchor = anchor;

        if (selectedAnchor != null)
        {
            //show visual stuff
            selectedAnchor.GetComponentInChildren<Renderer>().material.color = Color.red;
            anchorEditArrows.gameObject.SetActive(true);
            anchorEditArrows.transform.position = selectedAnchor.transform.position + Vector3.up * 0.01f;
            anchorEditArrows.transform.parent = selectedAnchor.transform;

            previewRuler.gameObject.SetActive(true);
            if (selectedAnchor.NextAnchor != null)
                UpdatePreviewRuler(selectedAnchor.NextAnchor.transform.position, selectedAnchor.transform.position);
            else
                UpdatePreviewRuler(selectedAnchor.PreviousAnchor.transform.position, selectedAnchor.transform.position);


            UnityMessageManager.Instance.SendMessageToFlutter("OnAnchorDeletionEnabled");
        }
        else
        {
            //hide visual stuff
            anchorEditArrows.transform.parent = null;
            anchorEditArrows.gameObject.SetActive(false);
            previewRuler.gameObject.SetActive(false);

            if (enterArObjectStateIfNull)
            {
                //goto arObject mode
                if (GlobalState.CurrentState == GlobalState.State.ARWallEdit)
                {
                    //UnityMessageManager.Instance.SendMessageToFlutter("OnAnchorCreationExit");
                    GlobalState.SetState(GlobalState.State.ARObject);
                }
                //UnityMessageManager.Instance.SendMessageToFlutter("OnAnchorDeletionDisabled");
            }
        }
    }

    public void TryDeleteSelectedAnchor()
    {
        if (selectedAnchor == null)
            return;

        if (selectedAnchor.PreviousAnchor != null)
            Destroy(selectedAnchor.PreviousAnchor.gameObject);

        if (selectedAnchor.NextAnchor != null)
            Destroy(selectedAnchor.NextAnchor.gameObject);

        foreach (var item in selectedAnchor.ConnectedWalls)
        {
            Destroy(item.gameObject);
        }

        anchorEditArrows.transform.parent = null;
        anchorEditArrows.gameObject.SetActive(false);
        previewRuler.gameObject.SetActive(false);

        Destroy(selectedAnchor.gameObject);

        selectedAnchor = null;
        //UnityMessageManager.Instance.SendMessageToFlutter("OnAnchorCreationExit");
        //GlobalState.SetState(GlobalState.State.ARObject);
    }
}
