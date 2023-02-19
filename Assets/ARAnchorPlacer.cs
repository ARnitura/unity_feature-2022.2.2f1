using FlutterUnityIntegration;
using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARAnchorPlacer : MonoBehaviour
{

    [SerializeField]
    private GameObject placerIndicator;

    [SerializeField]
    private ARWallAnchor anchorPrefab;

    [SerializeField]
    private ARWallObject wallPrefab;

    [SerializeField]
    private GameObject previewRuler;

    [SerializeField]
    private Transform previewAnchor;

    [SerializeField]
    private ARWallObject previewWall;




    private ARRaycastManager m_RaycastManager;
    private ARWallAnchor previousAnchor = null;
    private ARWallAnchor selectedAnchor;

    private enum InputState { None, Down, Pressed, Up }
    private InputState currentInputState;



    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        GlobalState.StateChanged += GlobalState_StateChanged;
        previewWall.InitEmpty();
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        if (obj == GlobalState.State.ARWallCreation)
        {
            //enabled = true;
            previousAnchor = null;
            placerIndicator.SetActive(true);
            HideCreationPreview();
        }
        else
        {

            //enabled = false;
            placerIndicator.SetActive(false);
            HideCreationPreview();
        }

        if (obj == GlobalState.State.ARWallEdit && GlobalState.PreviousState == GlobalState.State.ARWallCreation)
        {
            HideCreationPreview();
            GetComponent<ARAnchorEditor>().SelectAnchor(previousAnchor);
        }
        else
        {
            //GetComponent<ARAnchorEditor>().SelectAnchor(null);
        }




    }



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

        if (GlobalState.CurrentState == GlobalState.State.ARWallCreation)
        {
            switch (currentInputState)
            {
                case InputState.None:
                    break;
                case InputState.Down:

                    //spawn first anchor
                    //previousAnchor = null;
                    TryCreateAnchorAtScreenCenter();
                    break;
                case InputState.Pressed:
                    //display preview anchor
                    DisplayCreationPreview();
                    break;
                case InputState.Up:
                    //finish wall creation and move to edit mode
                    TryCreateAnchorAtScreenCenter();
                    GlobalState.SetState(GlobalState.State.ARWallEdit);
                    break;
                default:
                    break;
            }
        }



    }



    private void HideCreationPreview()
    {
        previewRuler.gameObject.SetActive(false);
        previewAnchor.gameObject.SetActive(false);
        previewWall.gameObject.SetActive(false);
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

    private void DisplayCreationPreview()
    {
        previewRuler.gameObject.SetActive(true);
        previewAnchor.gameObject.SetActive(true);
        previewWall.gameObject.SetActive(true);



#if UNITY_EDITOR
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (previousAnchor != null)
            {
                previewAnchor.position = hit.point;
                previewWall.UpdateMeshFromPos(previousAnchor.transform.position, hit.point);
                UpdatePreviewRuler(previousAnchor.transform.position, hit.point);
            }
        }
#else
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            previewAnchor.position = hits[0].pose.position;
        }
#endif


    }

    public void TryCreateAnchorAtScreenCenter()
    {

#if UNITY_EDITOR
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            CreateAnchor(hit.point);
            //DisableVisual();
            //EnableVisual();
        }
#else
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            CreateAnchor(hits[0].pose.position);
        }
#endif
    }

    private void CreateAnchor(Vector3 pos)
    {
        ARWallAnchor anchor = Instantiate(anchorPrefab);

        if (previousAnchor != null)
        {
            ARWallObject wall = Instantiate(wallPrefab);
            anchor.Init(pos, previousAnchor, wall);
        }
        else
            anchor.Init(pos, null, null);


        previousAnchor = anchor;
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


}