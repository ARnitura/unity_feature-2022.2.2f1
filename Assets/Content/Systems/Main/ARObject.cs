using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ARObject : MonoBehaviour
{
    private Renderer[] modelRenderers;


    [Header("Debug")]
    [SerializeField]
    private Transform axisDebugPrefab;
    [SerializeField]
    private Transform colliderDebugPrefab;

    [Header("Materials")]
    [SerializeField]
    private Material referenceMaterialStandard;
    [SerializeField]
    private Material referenceMaterialForSecondUV;

    public Transform Model { get; private set; }



    private List<string> animationClips = new List<string>();



    public void Init(Transform modelTransform, ARObjectDecorator decorator)
    {
        Model = modelTransform;
        Model.localPosition = Vector3.zero;
        modelRenderers = Model.GetComponentsInChildren<Renderer>();


        CreateModelCollider();
        LoadAnimations();
        SetMaterialsForMeshes();
        decorator.Decorate(Model);

        gameObject.SetActive(false);
    }

    public void Clear()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        WorldPosButtonsManager.Instance.RemoveButton(Model);
        animationClips = new List<string>();

        List<GameObject> toDestroy = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
            toDestroy.Add(transform.GetChild(i).gameObject);

        foreach (var item in toDestroy)
            Destroy(item);

        //gameObject.SetActive(true);
    }

    private void LoadAnimations()
    {
        Animation anim = Model.GetComponentInChildren<Animation>();
        if (anim)
        {
            //stop all movements and get animation clips info
            anim.playAutomatically = false;
            anim.Stop();
            anim.Rewind();
            anim.wrapMode = WrapMode.Once;
            anim.clip = null;



            string activateClip = "";
            string deactivateClip = "";


            //TODO: move parser logic
            foreach (AnimationState item in anim)
            {
                animationClips.Add(item.name);
                if (item.name.ToLower().Contains("unfold"))
                {
                    activateClip = item.name;
                    continue;
                }
                else if (item.name.ToLower().Contains("fold"))
                {
                    deactivateClip = item.name;
                }
            }



            WorldPosButtonsManager.Instance.AddToggleButton(Model, () =>
            {
                anim.Play(activateClip, PlayMode.StopSameLayer);
            }
                , () =>
                {
                    anim.Play(deactivateClip, PlayMode.StopSameLayer);
                });

            //create animation button
        }
    }
    public void ApplyTextures(List<Texture2DInfo> textures)
    {
        List<Material> appliedMaterials = new List<Material>();

        foreach (Renderer renderer in modelRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (appliedMaterials.Contains(material))
                    continue;

                bool materialApplied = false;
                foreach (Texture2DInfo texInfo in textures)
                    if (texInfo.TryApplyToMaterial(material))
                        materialApplied = true;

                if (materialApplied)
                    appliedMaterials.Add(material);
            }
        }

        #region DEBUG

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        int appliedTexturesCount = textures.Where(i => i.WasApplied).Count();
        if (appliedTexturesCount != textures.Count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Loaded {appliedTexturesCount}/{textures.Count} textures:");
            stringBuilder.AppendLine($"Loose textures:");

            foreach (Texture2DInfo item in textures.Where(i => i.WasApplied == false))
                stringBuilder.AppendLine(item.ToString());

            Debug.LogError(stringBuilder.ToString());
        }
        else
            Debug.LogWarning("All textures loaded");

        List<Material> uniqueMaterials = new List<Material>();
        foreach (Renderer renderer in modelRenderers)
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
        #endregion
    }


    private void SetMaterialsForMeshes()
    {
        foreach (Renderer renderer in modelRenderers)
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
    private void CreateModelCollider()
    {
        BoxCollider modelCollider = Model.gameObject.AddComponent<BoxCollider>();

        //save initial object rotation
        Quaternion currentRotation = Model.transform.rotation;

        //reset object rot
        Model.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Bounds resultingBounds = new Bounds(Model.transform.position, Vector3.zero);


        foreach (Renderer renderer in modelRenderers)
            resultingBounds.Encapsulate(renderer.bounds);

        //calculate localCenter
        Vector3 localCenter = resultingBounds.center - Model.transform.position;
        resultingBounds.center = localCenter;

        //apply init obj rotation
        Model.transform.rotation = currentRotation;

        //apply bounds to box collider
        modelCollider.center = resultingBounds.center;
        modelCollider.size = resultingBounds.size;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Transform debugCollider = Instantiate(colliderDebugPrefab, transform.position, transform.rotation);
        debugCollider.SetParent(transform);
        debugCollider.position = transform.position + resultingBounds.center;
        debugCollider.localScale = resultingBounds.size;


        Transform debugAxis = Instantiate(axisDebugPrefab, transform.position, transform.rotation);
        debugAxis.SetParent(transform);
        debugAxis.forward = Model.forward;
#endif
    }

}
