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
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning("Start AR call");
#endif

        Application.targetFrameRate = 60;
        session.subsystem.Start();
        aRPlaneManager.subsystem.Start();

        objectPlacer.ResetObject();
        objectPlacer.EnableVisual();
        objectLoader.ClearObject();
        aRPlaneManager.enabled = true;
        // aRPlaneManager.trackables.

    }

    public void ClearAR() // USED FROM FLUTTER
    {

        //StartAR();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning("Clear AR call");
#endif

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

    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [SerializeField]
    TextMeshProUGUI debugText;
#endif

    
    private void Update()
    {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
        string planeDetectionColor = aRPlaneManager.enabled? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        debugText.text = $"AR Plane Detection: <color=#{planeDetectionColor}>{aRPlaneManager.enabled}</color>\n" +
                         $"AR Plane trackables: {aRPlaneManager.trackables.count}";
#endif
    }


    float lastModelLoadCallTime = -2;
    public void LoadModel(string filePath)
    {
        //bycycle for multiple model loading
        if (Time.unscaledTime >= lastModelLoadCallTime + 1)
        {
            objectLoader.LoadModel(filePath);
            lastModelLoadCallTime = Time.unscaledTime;
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        else
        {
            Debug.LogWarning("Blocked extra loadmodel call");
        }
#endif
    }

    public void LoadTexture(string allPath)
    {
        //todo: better async await 
        objectLoader.StartCoroutine(objectLoader.LoadTextures(allPath));
    }
}
