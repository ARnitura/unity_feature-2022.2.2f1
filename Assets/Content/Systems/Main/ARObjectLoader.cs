using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TriLibCore;
using UnityEngine;

public class ARObjectLoader : MonoBehaviour
{
    [Header("Import")]
    [SerializeField]
    private bool useMainThread = false;
    [SerializeField]
    private AssetLoaderOptions assetLoaderOptions;
    [SerializeField]
    private Material referenceMaterialStandard;
    [SerializeField]
    private Material referenceMaterialForSecondUV;

    [Header("Decorator")]
    [SerializeField]
    private ARObjectDecorator decorator;

    [Header("Debug")]
    [SerializeField]
    private Transform debugModelPrefab;
    [SerializeField]
    private Transform axisDebugPrefab;
    [SerializeField]
    private Transform colliderDebugPrefab;


    public Transform LoadedModelTransform { get; private set; }

    private Renderer[] meshRenderers;


    //for debug purposes
    private string lastModelPath, lastTexPath;

    public static ARObjectLoader Instance { get; private set; }

    private List<Texture2DInfo> loadedTextures = new List<Texture2DInfo>();
    private void Start() => Instance = this;

    public void LoadModel(string filePath)
    {
        lastModelPath = filePath;

        //destroy previously loaded model
        if (LoadedModelTransform)
            Destroy(LoadedModelTransform.gameObject);



#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.Log($"Requesting model from {filePath}");

        if (!File.Exists(filePath))
            Debug.LogError($"Model filePath is not valid! (file doesn't exist)");
#endif
        if (useMainThread)
        {
            AssetLoaderContext assetLoaderContext = AssetLoader.LoadModelFromFileNoThread(filePath, null, null, assetLoaderOptions, null);
            OnModelLoaded(assetLoaderContext);
        }
        else
        {
            AssetLoader.LoadModelFromFile(filePath, null,
                delegate (AssetLoaderContext assetLoaderContext)
            {
                OnModelLoaded(assetLoaderContext);
            });

        }
    }


    private void OnModelLoaded(AssetLoaderContext assetLoaderContext)
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (assetLoaderContext.RootGameObject == null)
            Debug.LogError($"Loaded root gameobject is null (check path to model)");
#endif
        Transform newObject = new GameObject("LoadedModel").transform;
        LoadedModelTransform = assetLoaderContext.RootGameObject.transform;

        LoadedModelTransform.parent = newObject;

        CreateModelCollider();
        decorator.Decorate(LoadedModelTransform);
        LoadedModelTransform.gameObject.SetActive(false);

        UnityMessageManager.Instance.SendMessageToFlutter("ar_model_loaded");


#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning($"Model loaded <{LoadedModelTransform.name}>");
#endif
    }

    private void CreateModelCollider()
    {
        BoxCollider modelCollider = LoadedModelTransform.gameObject.AddComponent<BoxCollider>();

        //save initial object rotation
        Quaternion currentRotation = LoadedModelTransform.transform.rotation;

        //reset object rot
        LoadedModelTransform.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds resultingBounds = new Bounds(LoadedModelTransform.transform.position, Vector3.zero);

        meshRenderers = LoadedModelTransform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
            resultingBounds.Encapsulate(renderer.bounds);

        //calculate localCenter
        Vector3 localCenter = resultingBounds.center - LoadedModelTransform.transform.position;
        resultingBounds.center = localCenter;

        //apply init obj rotation
        LoadedModelTransform.transform.rotation = currentRotation;

        //apply bounds to box collider
        modelCollider.center = resultingBounds.center;
        modelCollider.size = resultingBounds.size;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Transform debugCollider = Instantiate(colliderDebugPrefab, LoadedModelTransform.position, LoadedModelTransform.rotation);
        debugCollider.position = resultingBounds.center;
        debugCollider.localScale = resultingBounds.size;
        debugCollider.SetParent(LoadedModelTransform);

        Transform debugAxis = Instantiate(axisDebugPrefab, LoadedModelTransform.position, LoadedModelTransform.rotation);
        debugAxis.SetParent(LoadedModelTransform);
        debugAxis.forward = LoadedModelTransform.forward;
#endif
    }

    public void LoadTextures(string allPath)
    {
        if (loadedTextures.Count != 0)
            foreach (Texture2DInfo item in loadedTextures)
            {
                item.Dispose();
            }
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        // loadedTextures = new List<Texture2DInfo>();
        StartCoroutine(LoadTexturesRoutine(allPath));
    }

    private void SetMaterialsForMeshes()
    {
        foreach (Renderer renderer in meshRenderers)
            foreach (Material material in renderer.materials)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;

                Mesh resultingMesh = null;

                if (meshFilter != null)
                {
                    resultingMesh = meshFilter.mesh;
                }
                else if (skinnedMeshRenderer != null)
                {
                    resultingMesh = skinnedMeshRenderer.sharedMesh;
                }
                else
                    throw new System.ArgumentNullException($"Couldn't apply material to object {renderer.gameObject.name}! Check if model loaded correctly");

                if (resultingMesh != null)
                {
                    if (resultingMesh.uv2.Length > 0)
                    {
                        //wow, we have second uv set...
                        material.shader = referenceMaterialForSecondUV.shader;
                        material.CopyPropertiesFromMaterial(referenceMaterialForSecondUV);
                    }
                    else
                    {
                        material.shader = referenceMaterialStandard.shader;
                        material.CopyPropertiesFromMaterial(referenceMaterialStandard);
                    }
                }

            }
    }

    private IEnumerator LoadTexturesRoutine(string allPath)
    {
        lastTexPath = allPath;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.Log($"Requesting textures from {allPath}");
        if (!LoadedModelTransform)
            Debug.LogError("Illegal LoadTextures call on null gameObject");
#endif


        //load all maps
        loadedTextures = Texture2DInfo.GetTexturesFromCombinedPath(allPath);
        //int mapCount = textures.Count;
        //int loadedTex = 0;
        List<Material> appliedMaterials = new List<Material>();

        //copy material properties from ref material
        SetMaterialsForMeshes();


        //skip the frame to apply material properties
        yield return null;

        //do something reasonable with this bycycle
        foreach (MeshRenderer renderer in meshRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (appliedMaterials.Contains(material))
                    continue;

                string materialPrefix = material.name.Split('.', '_')[0].ToLower();
                bool materialApplied = false;
                foreach (Texture2DInfo texInfo in loadedTextures)
                {
                    if (texInfo.TryApplyToMaterial(material))
                    {
                        materialApplied = true;
                        //loadedTex++;
                    }
                }
                if (materialApplied)
                {
                    appliedMaterials.Add(material);
                }
            }
        }



#if DEVELOPMENT_BUILD || UNITY_EDITOR
        int appliedTexturesCount = loadedTextures.Where(i => i.WasApplied).Count();
        if (appliedTexturesCount != loadedTextures.Count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Loaded {appliedTexturesCount}/{loadedTextures.Count} textures:");
            stringBuilder.AppendLine($"Loose textures:");

            foreach (Texture2DInfo item in loadedTextures.Where(i => i.WasApplied == false))
                stringBuilder.AppendLine(item.ToString());

            Debug.LogError(stringBuilder.ToString());
        }
        else
            Debug.LogWarning("All textures loaded");

        List<Material> uniqueMaterials = new List<Material>();
        foreach (MeshRenderer renderer in meshRenderers)
            foreach (Material material in renderer.materials)
                if (!uniqueMaterials.Contains(material))
                    uniqueMaterials.Add(material);

        if (appliedMaterials.Count != uniqueMaterials.Count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Applied {appliedMaterials.Count}/{uniqueMaterials.Count} materials:");
            stringBuilder.AppendLine($"Loose materials:");

            foreach (Material looseMaterial in uniqueMaterials.Except(appliedMaterials).ToList())
            {
                stringBuilder.AppendLine($"Name: {looseMaterial.name}");
            }


            Debug.LogError(stringBuilder.ToString());
        }
        else
        {
            Debug.LogWarning("All materials loaded");
        }
#endif
    }




#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void DebugReloadModel() => LoadModel(lastModelPath);
    public void DebugReloadTextures()
    {
        if (LoadedModelTransform != null)
            LoadTextures(lastTexPath);
        else
            Debug.LogError("Tried to load textures before loading model");
    }
    public void DebugStandaloneLoadModel()
    {
        if (LoadedModelTransform)
            Destroy(LoadedModelTransform.gameObject);

        LoadedModelTransform = Instantiate(debugModelPrefab, Vector3.zero, Quaternion.identity);

        Transform newObject = new GameObject("LoadedModel").transform;
        LoadedModelTransform.parent = newObject;

        CreateModelCollider();
        decorator.Decorate(LoadedModelTransform);
        LoadedModelTransform.gameObject.SetActive(false);

        UnityMessageManager.Instance.SendMessageToFlutter("ar_model_loaded");

        Debug.LogWarning("Loaded model from resources");
    }
#endif
}
