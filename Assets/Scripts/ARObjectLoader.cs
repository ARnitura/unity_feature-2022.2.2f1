using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using TriLibCore;
using UnityEngine;

public class ARObjectLoader : MonoBehaviour
{
    readonly Vector3 spawnPoint = new Vector3(15, 15, 15);

    [SerializeField]
    Transform debugModelPrefab;


    [SerializeField]
    Transform shadowPlanePrefab;
    //spawned object
    public Transform SpawnedObject {
        get;
        private set; 
    }
    [SerializeField]
    AssetLoaderOptions assetLoaderOptions; 
    //private Transform spawnedObjectInstance;

    

    //for debug purposes
    string lastModelPath, lastTexPath;

    //some time... some day...
    //[SerializeField]
    //Material referenceMaterial;

    [SerializeField]
    Transform axisPrefab;
    [SerializeField]
    Transform colliderDebugPrefab;
    [SerializeField]
    Transform axisDebugPrefab;

    MeshRenderer[] meshRenderers;

    [SerializeField]
    Material referenceMaterial;

    [SerializeField]
    bool rulerEnabled = false;
    Transform rulerRoot;

    [SerializeField]
    UnityMessageManager unityMessageManager;

    [Obsolete("No need in calling that - LoadModel clears old model if necessary")]
    public void ClearObject()
    {
        if (SpawnedObject)
            Destroy(SpawnedObject.gameObject);
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        else
            Debug.LogWarning("Warning: Trying to clear empty scene!");
        #endif
    }

    public void SwitchRuler()
    {
        rulerEnabled = !rulerEnabled;
        rulerRoot?.gameObject.SetActive(rulerEnabled);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning("Ruler " + (rulerEnabled? "on" : "off"));
#endif
    }

    public void LoadModel(string filePath)
    {
        if (SpawnedObject)
            Destroy(SpawnedObject.gameObject);

        lastModelPath = filePath;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.Log($"loading model from {filePath}");
#endif

        AssetLoaderContext assetLoaderContext = AssetLoader.LoadModelFromFileNoThread(filePath, null, null, assetLoaderOptions, null);
        SpawnedObject = assetLoaderContext.RootGameObject.gameObject.transform;

        CreateBoxCollider();
        CreateAxisSizesAndShadowPlane();

        unityMessageManager.SendMessageToFlutter("ar_model_loaded");

        SpawnedObject.gameObject.SetActive(false);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning($"Model loaded GO_name= {SpawnedObject.name}");
#endif

        /*
        AssetLoader.LoadModelFromFile(filePath, 
            null,
            delegate (AssetLoaderContext assetLoaderContext)
            {

            SpawnedObject = assetLoaderContext.RootGameObject.gameObject.transform;

            CreateBoxCollider();
            CreateAxisSizesAndShadowPlane();
            
            unityMessageManager.SendMessageToFlutter("ar_model_loaded");
            
            SpawnedObject.gameObject.SetActive(false);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning($"Model loaded GO_name= {SpawnedObject.name}");
#endif
        }, 
        null, null, null, assetLoaderOptions, null);
        */
    }

    void CreateAxisSizesAndShadowPlane()
    {


        rulerRoot = new GameObject("RulerRoot").transform;
        rulerRoot.position = SpawnedObject.position;
        rulerRoot.SetParent(SpawnedObject);

        //create up axis
        Transform upAxis = Instantiate(axisPrefab);
        Transform rightAxis = Instantiate(axisPrefab);
        Transform forwardAxis = Instantiate(axisPrefab);



        Transform shadowPlane = Instantiate(shadowPlanePrefab);

        BoxCollider modelCollider = SpawnedObject.gameObject.GetComponent<BoxCollider>();


        
        //min corner world space coord
        Vector3 minCornerPos = SpawnedObject.position + modelCollider.bounds.min;
        Vector3 colliderSize = modelCollider.size;

        upAxis.position = minCornerPos + Vector3.up * colliderSize.y / 2;
        forwardAxis.position = minCornerPos + Vector3.right * colliderSize.x + Vector3.forward * colliderSize.z/2;
        rightAxis.position = minCornerPos + Vector3.right * colliderSize.x/2;


        upAxis.rotation = Quaternion.Euler(0, -90, -90);
        forwardAxis.rotation = Quaternion.Euler(90, 0, 90);
        rightAxis.rotation = Quaternion.Euler(90, 0, 0);

        //omg just invent something better, bc it is not stable and depends on texture dimensions
        upAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.y*2, 1f);
        forwardAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.z*2, 1f);
        rightAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.x*2, 1f);

        upAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.y*100)} cm";
        forwardAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.z * 100)} cm";
        rightAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.x * 100)} cm";

        upAxis.SetParent(rulerRoot);
        rightAxis.SetParent(rulerRoot);
        forwardAxis.SetParent(rulerRoot);

        shadowPlane.position = SpawnedObject.position + modelCollider.center + Vector3.down * colliderSize.y / 1.98f;

        shadowPlane.SetParent(SpawnedObject);

        rulerRoot.gameObject.SetActive(rulerEnabled);

    }
    void CreateBoxCollider()
    {
        BoxCollider modelCollider = SpawnedObject.gameObject.AddComponent<BoxCollider>();

        //save initial object rotation
        Quaternion currentRotation = SpawnedObject.transform.rotation;

        //reset object rot
        SpawnedObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds resultingBounds = new Bounds(SpawnedObject.transform.position, Vector3.zero);

        meshRenderers = SpawnedObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
            resultingBounds.Encapsulate(renderer.bounds);
        
        //calculate localCenter
        Vector3 localCenter = resultingBounds.center - SpawnedObject.transform.position;
        resultingBounds.center = localCenter;

        //apply init obj rotation
        SpawnedObject.transform.rotation = currentRotation;

        //apply bounds to box collider
        modelCollider.center = resultingBounds.center;
        modelCollider.size = resultingBounds.size;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Transform debugCollider = Instantiate(colliderDebugPrefab, SpawnedObject.position, SpawnedObject.rotation);
            debugCollider.position = resultingBounds.center;
            debugCollider.localScale = resultingBounds.size;
            debugCollider.SetParent(SpawnedObject);

            Transform debugAxis = Instantiate(axisDebugPrefab, SpawnedObject.position, SpawnedObject.rotation);
            debugAxis.SetParent(SpawnedObject);
            debugAxis.forward = SpawnedObject.forward;
#endif
    }

    public IEnumerator LoadTextures(string allPath)
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.Log($"loading textures from path {allPath}");
#endif
        lastTexPath = allPath;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (!SpawnedObject)
            Debug.LogError("spawnedObject nullpo on texture loading");
#endif




        //load all maps
        List<Texture2DInfo> textures = GetTexturesFromCombinedPath(allPath);
        int mapCount = textures.Count();
        int loadedTex = 0;

        //copy material properties from ref material
        foreach (var renderer in meshRenderers)
            foreach (var material in renderer.materials)
            {
                material.shader = referenceMaterial.shader;
                material.CopyPropertiesFromMaterial(referenceMaterial);
            }


        //skip the frame to apply material properties
        yield return null;

        //do something reasonable with this bycycle
        foreach (var renderer in meshRenderers)
        {
            foreach (var material in renderer.materials)
            {
                for (int k = 0; k < textures.Count; k++)
                {
                    string materialPrefix = material.name.Split('.')[0];

                    if (textures[k].Path.Contains(materialPrefix + "_BaseColor"))
                    {
                        material.SetTexture("_MainTex", textures[k].Texture);
                        loadedTex++;
                    }
                    else if (textures[k].Path.Contains(materialPrefix + "_Normal"))
                    {
                        material.SetTexture("_BumpMap", textures[k].Texture);
                        loadedTex++;
                    }
                    else if (textures[k].Path.Contains(materialPrefix + "_Specular"))
                    {
                        material.SetTexture("_SpecGlossMap", textures[k].Texture);
                        loadedTex++;
                    }
                    /*
                    else if (textures[k].Path.Contains(materialPrefix + "_MetallicGlossMap"))
                    {
                        material.SetTexture("_MetallicGlossMap", textures[k].Texture);
                        loadedTex++;
                    } 
                    */
                    else if (textures[k].Path.Contains(materialPrefix + "_AO"))
                    {
                        material.SetTexture("_OcclusionMap", textures[k].Texture);
                        loadedTex++;
                    }
                    /*
                    else if (textures[k].Path.Contains(materialPrefix + "_EmissionMap"))
                    {
                        material.SetTexture("_EmissionMap", textures[k].Texture);
                        loadedTex++;
                    }
                    */
                }
            }
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (loadedTex != mapCount)
            Debug.LogWarning($"Loaded {loadedTex}/{mapCount} textures");
        else
            Debug.Log("Tex loading complete");
#endif
    }
    public static Texture2D LoadTextureData(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            
            if(!tex.LoadImage(fileData)) //..this will auto-resize the texture dimensions.
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError($"failed to load texture from path: {filePath}");
#endif
            }


        }

        return tex;
    }

    public static Texture2D LoadNormalData(string filePath)
    {
            Texture2D loadedNormalMap = LoadTextureData(filePath);
            // note format is now copied from the loaded texture
            Texture2D convertedNormalMap = new Texture2D(loadedNormalMap.width, loadedNormalMap.height, loadedNormalMap.format, true, true);

            // the documentation on this function says it's GPU side only, but this is a lie
            // this is roughly equivalent to:
            // convertedNormalMap.SetPixels32(loadedNormalMap.GetPixels32(0), 0);
            // if both textures are readable on the CPU, which they will be in this case
            Graphics.CopyTexture(loadedNormalMap, convertedNormalMap);

            convertedNormalMap.Apply();



        return convertedNormalMap;

    }

    public static List<Texture2DInfo> GetTexturesFromCombinedPath(string allPath)
    {
        List<Texture2DInfo> result = new List<Texture2DInfo>();
        List<string> maps = allPath.Split(", ").ToList();
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].Contains("_Normal"))
                result.Add(new Texture2DInfo(LoadNormalData(maps[i]), maps[i]));
            else 
                result.Add(new Texture2DInfo(LoadTextureData(maps[i]), maps[i]));
        }
        return result;
    }

    float Round1Digit(float value)
    {
        return Mathf.Round(value * 10) / 10;
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void DebugReloadModel()
    {
        ClearObject();
        LoadModel(lastModelPath);
    }
    public void DebugReloadTextures()
    {
        if (SpawnedObject != null)
            StartCoroutine(LoadTextures(lastTexPath));
        else
            Debug.LogError("Tried to load textures before loading model");
    }

    public void DebugStandaloneLoadModel()
    {
        
        SpawnedObject = Instantiate(debugModelPrefab, Vector3.zero, Quaternion.identity);
        CreateBoxCollider();
        CreateAxisSizesAndShadowPlane();
        SpawnedObject.gameObject.SetActive(false);
        Debug.LogWarning("Loaded model from resources");
    }
#endif
}
