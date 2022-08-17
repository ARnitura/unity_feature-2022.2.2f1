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

    //spawned object
    public Transform SpawnedObject {
        get;
        private set; 
    }

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


    private void Start()
    {
        assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        //spawnedObject = new GameObject("modelDummy");
    }

    public void ClearObject()
    {
        if (SpawnedObject)
            Destroy(SpawnedObject.gameObject);
        #if DEVELOPMENT_BUILD
        else
            
            Debug.LogWarning("Warning: Trying to clear empty scene!");
        #endif
    }


    public void LoadModel(string filePath)
    {
        lastModelPath = filePath;


#if DEVELOPMENT_BUILD
        Debug.Log($"loading model from {filePath}");
#endif
        /*
        AssetLoaderContext loadedModel = AssetLoader.LoadModelFromFileNoThread(filePath, null, null, assetLoaderOptions);
        spawnedObject = loadedModel.RootGameObject.gameObject;
        meshRenderers = spawnedObject.GetComponentsInChildren<MeshRenderer>();
        CreateBoxCollider();
        spawnedObject.SetActive(false);
        GetComponent<UnityMessageManager>().SendMessageToFlutter("Модель поставлена");
        Debug.LogWarning($"Model loaded GO_name= {spawnedObject.name}");
        */
        AssetLoader.LoadModelFromFile(filePath, 
            null,
            delegate (AssetLoaderContext assetLoaderContext)
            {

            SpawnedObject = assetLoaderContext.RootGameObject.gameObject.transform;
                

            CreateBoxCollider();
            CreateAxisSizes();
            
            GetComponent<UnityMessageManager>().SendMessageToFlutter("Модель поставлена");
            
            SpawnedObject.gameObject.SetActive(false);
#if DEVELOPMENT_BUILD
            Debug.LogWarning($"Model loaded GO_name= {SpawnedObject.name}");
#endif
        }, 
        null, null, null, assetLoaderOptions, null);
        
    }

    void CreateAxisSizes()
    {
        //create up axis
        Transform upAxis = Instantiate(axisPrefab);
        Transform rightAxis = Instantiate(axisPrefab);
        Transform forwardAxis = Instantiate(axisPrefab);

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

        upAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.y, 0.5f);
        forwardAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.z, 0.5f);
        rightAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.x, 0.5f);

        upAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.y*100)} cm";
        forwardAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.z * 100)} cm";
        rightAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.x * 100)} cm";

        upAxis.SetParent(SpawnedObject);
        forwardAxis.SetParent(SpawnedObject);
        rightAxis.SetParent(SpawnedObject);

    }
    void CreateBoxCollider()
    {
        BoxCollider modelCollider = SpawnedObject.gameObject.AddComponent<BoxCollider>();

        //save initial object rotation
        Quaternion currentRotation = SpawnedObject.transform.rotation;

        //reset object rot
        SpawnedObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds resultingBounds = new Bounds(SpawnedObject.transform.position, Vector3.zero);

        MeshRenderer[] meshRenderers = SpawnedObject.GetComponentsInChildren<MeshRenderer>();
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

#if DEVELOPMENT_BUILD
            Transform debugCollider = Instantiate(colliderDebugPrefab, SpawnedObject.position, SpawnedObject.rotation);
            debugCollider.position = resultingBounds.center;
            debugCollider.localScale = resultingBounds.size;
            debugCollider.SetParent(SpawnedObject);

            Transform debugAxis = Instantiate(axisDebugPrefab, SpawnedObject.position, SpawnedObject.rotation);
            debugAxis.SetParent(SpawnedObject);
            debugAxis.forward = SpawnedObject.forward;
#endif
    }

    public void LoadTextures(string allPath)
    {
#if DEVELOPMENT_BUILD
        Debug.Log($"loading textures from path {allPath}");
#endif
        lastTexPath = allPath;

#if DEVELOPMENT_BUILD
        if (!SpawnedObject)
            Debug.LogError("spawnedObject nullpo on texture loading");
#endif

        MeshRenderer[] meshRenderers = SpawnedObject.GetComponentsInChildren<MeshRenderer>();
        //Debug.Log($"got mesh renderers list");

        List<string> maps = allPath.Split(", ").ToList(); // Карты для мешей

        int mapCount = allPath.Split(", ").Count();
        int loadedTex = 0;
        foreach (var renderer in meshRenderers)
        {
            //Debug.Log($"trying renderer on {renderer.gameObject.name}");
            foreach (var material in renderer.materials)
            {
                //Debug.Log($"trying material {material.name}");
                // Debug.Log("check2");
                
                //Debug.Log("check3");
                for (int k = 0; k < maps.Count; k++) // Распределение текстур относительно материалов
                {
                    string materialPrefix = material.name.Split(".")[0];
                    //Debug.Log($"trying to fill material with name {materialPrefix}");
                    if (maps[k].Contains(materialPrefix + "_BaseColor"))
                    {
                        var BaseColor = LoadTextureData(maps[k]);
                        material.SetTexture("_MainTex", BaseColor);
                        //maps.RemoveAt(k);
                        loadedTex++;
                        //Debug.Log("_MainTex: successful");
                    } // Поиск BaseColor для материала
                    else if (maps[k].Contains(materialPrefix + "_Normal"))
                    {
                        var normalMap = LoadTextureData(maps[k]);
                        material.SetTexture("_BumpMap", normalMap);
                        //maps.RemoveAt(k);
                        loadedTex++;
                        //Debug.Log("_BumpMap: successful");
                    } // Поиск Normal для материала


                    // we don't need heightmaps
                    /*
                    else if (maps[k].Contains(find_material + "_Height"))
                    {
                        var HeightMap = LoadTextureData(maps[k]);
                        comp.material.SetTexture("_Height", HeightMap);
                        maps.RemoveAt(k);
                        Debug.Log("_Height: successful");
                    } // Поиск HeightMap для материала
                    */

                    else if (maps[k].Contains(materialPrefix + "_MetallicGlossMap"))
                    {
                        var MetallicGlossMap = LoadTextureData(maps[k]);
                        material.SetTexture("_MetallicGlossMap", MetallicGlossMap);
                        //maps.RemoveAt(k);
                        loadedTex++;
                        //Debug.Log("_MetallicGlossMap: successful");
                    } // Поиск _MetallicGlossMap для материала
                    else if (maps[k].Contains(materialPrefix + "_OcclusionMap"))
                    {
                        var OcclusionMap = LoadTextureData(maps[k]);
                        material.SetTexture("_OcclusionMap", OcclusionMap);
                        //maps.RemoveAt(k);
                        loadedTex++;
                        //Debug.Log("_OcclusionMap: successful");
                    } // Поиск _OcclusionMap для материала
                    else if (maps[k].Contains(materialPrefix + "_EmissionMap"))
                    {
                        var EmissionMap = LoadTextureData(maps[k]);
                        material.SetTexture("_EmissionMap", EmissionMap);
                        //maps.RemoveAt(k);
                        loadedTex++;
                        //Debug.Log("_EmissionMap: successful");
                    } // Поиск _EmissionMap для материала


                    // we already have glossiness in metallic map
                    /*
                    else if (maps[k].Contains(find_material + "_Glossiness"))
                    {
                        var GlossinessMap = LoadTextureData(maps[k]);
                        comp.material.SetTexture("_Glossiness", GlossinessMap);
                        maps.RemoveAt(k);
                        Debug.Log("_Glossiness: successful");
                    }
                    */
                }
            }

            // EXAMPLE: LoadTexture(["Wood_BaseColor"])
        }
#if DEVELOPMENT_BUILD
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
#if DEVELOPMENT_BUILD
                Debug.LogError($"failed to load texture from path: {filePath}");
#endif
            }


        }

        return tex;
    }

    float Round1Digit(float value)
    {
        return Mathf.Round(value * 10) / 10;
    }

#if DEVELOPMENT_BUILD
    public void DebugReloadModel()
    {
        ClearObject();
        LoadModel(lastModelPath);
    }
    public void DebugReloadTextures()
    {
        if (SpawnedObject != null)
            LoadTextures(lastTexPath);
        else
            Debug.LogError("Tried to load textures before loading model");
    }

    public void DebugStandaloneLoadModel()
    {
        SpawnedObject = Instantiate(debugModelPrefab);
        CreateBoxCollider();
        CreateAxisSizes();
        SpawnedObject.gameObject.SetActive(false);
    }
#endif
}
