using UnityEngine;
using UnityEngine.XR.ARFoundation;
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
    private ARSession session;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [SerializeField]
    [Header("Debug")]
    private TextMeshProUGUI debugText;
#endif

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
#if !UNITY_EDITOR
        session.subsystem.Start();
        aRPlaneManager.subsystem.Start();
#endif
        objectPlacer.ResetObject();
        //objectPlacer.EnableVisual();
        //objectLoader.ClearObject();
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




    private void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        string planeDetectionColor = aRPlaneManager.enabled ? ColorUtility.ToHtmlStringRGB(Color.green) : ColorUtility.ToHtmlStringRGB(Color.red);
        debugText.text = $"AR Plane Detection: <color=#{planeDetectionColor}>{aRPlaneManager.enabled}</color>\n" +
                         $"AR Plane trackables: {aRPlaneManager.trackables.count}";
#endif
    }


    public void LoadModel(string filePath)
    {
        ARObjectLoader.Instance.LoadModel(filePath);
    }

    public void LoadTexture(string allPath)
    {
        ARObjectLoader.Instance.LoadTextures(allPath);
    }
}
