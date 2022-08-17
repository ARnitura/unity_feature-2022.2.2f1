using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;
using TriLibCore;
using TriLibCore.General;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FlutterMessages : MonoBehaviour
{
    [SerializeField]
    ARObjectLoader objectLoader;

    [SerializeField]
    ARPlaneManager aRPlaneManager;

    [SerializeField]
    ARObjectPlacer objectPlacer;

    [SerializeField]
    ARSession session;

    private void Start()
    {
        Application.targetFrameRate = 60;
        //StartAR();
    }

    public void StartAR()
    {

        Application.targetFrameRate = 60;
        session.subsystem.Start();
        aRPlaneManager.subsystem.Start();

        objectPlacer.ResetObject();
        objectPlacer.EnableVisual();
        objectLoader.ClearObject();
        aRPlaneManager.enabled = true;
        // aRPlaneManager.trackables.
        
        Debug.LogWarning("Start AR call");
    }

    public void ClearAR() // USED FROM FLUTTER
    {

#if UNITY_IOS
        if (session.subsystem is ARKitSessionSubsystem subsystem)
        {
            subsystem.ApplyWorldMap(default);
        }
#endif

        session.Reset();

        session.subsystem.Stop();
        aRPlaneManager.subsystem.Stop();

        objectPlacer.ResetObject();
        objectPlacer.EnableVisual();
        objectLoader.ClearObject();
        aRPlaneManager.enabled = false;
        Application.targetFrameRate = 5;
        //StartAR();
        Debug.LogWarning("Clear AR call");
    }


    [SerializeField]
    TextMeshProUGUI debugText; 
    private void Update()
    {

        string planeDetectionColor = aRPlaneManager.enabled? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        debugText.text = $"AR Plane Detection: <color=#{planeDetectionColor}>{aRPlaneManager.enabled}</color>\n" +
                         $"AR Plane trackables: {aRPlaneManager.trackables.count}";
    }

    public void LoadModel(string filePath)
    {
        objectLoader.LoadModel(filePath);
    }

    public void LoadTexture(string allPath)
    {
        objectLoader.LoadTextures(allPath);
    }
}
