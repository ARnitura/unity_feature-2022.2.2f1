using Cysharp.Threading.Tasks;
using FlutterUnityIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TriLibCore;
using TriLibCore.Interfaces;
using UnityEngine;

public class ARObjectLoader : MonoBehaviour
{
    [Header("Import")]
    [SerializeField]
    private AssetLoaderOptions assetLoaderOptions;


    [Header("Decorator")]
    [SerializeField]
    private ARObjectDecorator decorator;

    [Header("Placer")]
    [SerializeField]
    private ARObjectPlacer placer;


    [Header("UI")]
    [SerializeField]
    private ScanManager scanManager;

    [Header("Debug")]
    [SerializeField]
    private Transform debugModelPrefab;

    public ARObject ARObject { get; private set; }

    [SerializeField]
    private ARObject arObjectPrefab;

    private List<Texture2DInfo> loadedTextures = new List<Texture2DInfo>();
    private bool modelLoaded = false;

    public Action onModelLoaded;


    //model loading
    public void LoadModel(string filePath)
    {
        modelLoaded = false;
        //destroy previously loaded model
        if (ARObject != null)
            ARObject.Clear();
        else
            ARObject = Instantiate(arObjectPrefab, Vector3.zero, Quaternion.identity);

        placer.ResetObject();

        #region debug
#if true || UNITY_EDITOR
        Debug.Log($"Requesting model from {filePath}");

        if (!File.Exists(filePath))
            Debug.LogError($"Model filePath is not valid! (file doesn't exist)");
#endif
        #endregion

        /*
        if (useMainThread)
        {
            AssetLoaderContext assetLoaderContext = AssetLoader.LoadModelFromFileNoThread(filePath, null, null, assetLoaderOptions, null);
            OnModelLoaded(assetLoaderContext);
        }
        */
        AssetLoader.LoadModelFromFile(filePath, null,
            delegate (AssetLoaderContext assetLoaderContext) { OnModelLoaded(assetLoaderContext); },
            delegate (AssetLoaderContext context, float progrss) { UnityMessageManager.Instance.SendMessageToFlutter($"{{\"percentLoading\": {Mathf.RoundToInt(progrss * 100)}}}"); },
            delegate (IContextualizedError context) { Debug.LogError($"Failed to load 3d model: {context.GetInnerException().Message}"); },
            ARObject.gameObject,
            assetLoaderOptions);

    }
    private void OnModelLoaded(AssetLoaderContext assetLoaderContext)
    {
#if true || UNITY_EDITOR
        if (assetLoaderContext.RootGameObject.transform == null)
        {
            Debug.LogError($"Loaded root gameobject is null (check path to model)");
            return;
        }
#endif

        ARObject.Init(assetLoaderContext.RootGameObject.transform, decorator);
        UnityMessageManager.Instance.SendMessageToFlutter("ar_model_loaded");
        modelLoaded = true;
        placer.SelectObject(ARObject);

        GlobalState.SetState(GlobalState.State.Scan);
        //scanManager.StartScan();


#if true || UNITY_EDITOR
        Debug.LogWarning($"Model loaded <{assetLoaderContext.RootGameObject.name}>");
#endif
        onModelLoaded?.Invoke();

    }

    public async void LoadTextures(List<string> paths)
    {
        if (loadedTextures.Count != 0)
            foreach (Texture2DInfo item in loadedTextures)
                item.Dispose();

        System.GC.Collect();
        await Resources.UnloadUnusedAssets();

        #region DEBUG
#if true || UNITY_EDITOR
        Debug.Log($"Requesting textures from {paths}");
        if (!modelLoaded)
        {
            Debug.LogError("Illegal LoadTextures call on null gameObject");
            return;
        }
#endif
        #endregion

        loadedTextures = Texture2DInfo.GetTexturesFromPaths(paths);
        ARObject.ApplyTextures(loadedTextures);
    }

#if true || UNITY_EDITOR
    public void DebugStandaloneLoadModel()
    {
        FindObjectOfType<FlutterMessagesReciever>().StartAR();

        if (ARObject != null)
            ARObject.Clear();
        else
            ARObject = Instantiate(arObjectPrefab, Vector3.one * 9999, Quaternion.identity);




        Transform model = Instantiate(debugModelPrefab, Vector3.zero, Quaternion.identity);
        model.SetParent(ARObject.transform);
        ARObject.Init(model, decorator);



        UnityMessageManager.Instance.SendMessageToFlutter("ar_model_loaded");
        modelLoaded = true;
        placer.SelectObject(ARObject);

        GlobalState.SetState(GlobalState.State.Scan);

        onModelLoaded?.Invoke();
        Debug.LogWarning("Loaded model from resources");
    }
#endif
}
