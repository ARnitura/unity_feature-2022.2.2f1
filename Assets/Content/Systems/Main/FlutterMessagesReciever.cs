using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System.Linq;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
using TMPro;

public class FlutterMessagesReciever : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager aRPlaneManager;

    [SerializeField]
    private ARObjectPlacer objectPlacer;


    [SerializeField]
    private ScanManager helpMessages;

    [SerializeField]
    private ARSession session;


    [SerializeField]
    private ARObjectLoader objectLoader;


    private void Start()
    {
        ClearAR();
        //Application.targetFrameRate = 60;//StartAR();
    }

    public void StartAR()
    {
#if true || UNITY_EDITOR


        Debug.LogWarning("Start AR call");
#endif

        Application.targetFrameRate = 60;
#if !UNITY_EDITOR
        session.subsystem.Start();
        
#endif
        GlobalState.SetState(GlobalState.State.None);
        objectPlacer.ResetObject();
        //objectPlacer.EnableVisual();
        //objectLoader.ClearObject();
        //aRPlaneManager.enabled = true;
        // aRPlaneManager.trackables.
        //helpMessages.StartScan();
    }

    public void ClearAR() // USED FROM FLUTTER
    {

        //StartAR();
#if true || UNITY_EDITOR
        Debug.LogWarning("Clear AR call");
#endif

#if UNITY_IOS
        if (session.subsystem is ARKitSessionSubsystem subsystem)
        {
            subsystem.ApplyWorldMap(default);
        }
#endif
#if !UNITY_EDITOR
        session.Reset();

        session.subsystem.Stop();
        aRPlaneManager.subsystem.Stop();
#endif
        objectPlacer.ResetObject();
        //objectPlacer.EnableVisual();
        //objectLoader.ClearObject();
        aRPlaneManager.enabled = false;
        Application.targetFrameRate = 5;

    }

    public void EnableAnchorMode()
    {
        GlobalState.SetState(GlobalState.State.ARWallCreation);
    }

    public void DisableAnchorMode()
    {
        GlobalState.SetState(GlobalState.PreviousState);
    }



    public void LoadModel(string filePath)
    {
        objectLoader.LoadModel(filePath);
    }

    public void LoadTexture(string allPath)
    {
        List<string> splitted = allPath.Split(", ").ToList();
        objectLoader.LoadTextures(splitted);
    }


    public void EnterAnchorCreation()
    {
        GlobalState.SetState(GlobalState.State.ARWallCreation);
    }

    public void TryDeleteSelectedAnchor()
    {
        FindObjectOfType<ARAnchorPlacer>().TryDeleteSelectedAnchor();
    }
}
