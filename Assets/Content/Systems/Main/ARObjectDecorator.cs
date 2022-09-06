using TMPro;
using UnityEngine;

public class ARObjectDecorator : MonoBehaviour
{
    [Header("Ruler")]
    [SerializeField]
    private Transform rulerPrefab;
    [field: SerializeField]
    public bool RulerEnabled { get; private set; } = false;
    private Transform rulerRoot;


    [Header("Other")]
    [SerializeField]
    private Transform shadowPlanePrefab;




    private Transform modelTransform;
    private BoxCollider modelCollider;

    public void SwitchRuler()
    {
        RulerEnabled = !RulerEnabled;
        rulerRoot?.gameObject.SetActive(RulerEnabled);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogWarning("Ruler " + (RulerEnabled ? "on" : "off"));
#endif
    }

    public void Decorate(Transform modelTransform)
    {
        this.modelTransform = modelTransform;
        CreateRulers();
        CreateShadowplane();
    }

    private void CreateShadowplane()
    {
        Transform shadowPlane = Instantiate(shadowPlanePrefab);

        shadowPlane.position = modelTransform.position + modelCollider.center + Vector3.down * modelCollider.size.y / 1.98f;

        shadowPlane.SetParent(modelTransform.parent);
    }

    //TODO move ruler creation logic to individual rulers
    private void CreateRulers()
    {
        rulerRoot = new GameObject("RulerRoot").transform;
        rulerRoot.position = modelTransform.position;
        rulerRoot.SetParent(modelTransform.parent);

        //create up axis
        Transform upAxis = Instantiate(rulerPrefab);
        Transform rightAxis = Instantiate(rulerPrefab);
        Transform forwardAxis = Instantiate(rulerPrefab);





        modelCollider = modelTransform.gameObject.GetComponent<BoxCollider>();



        //min corner world space coord
        Vector3 minCornerPos = modelTransform.position + modelCollider.bounds.min;
        Vector3 colliderSize = modelCollider.size;

        upAxis.position = minCornerPos + Vector3.up * colliderSize.y / 2;
        forwardAxis.position = minCornerPos + Vector3.right * colliderSize.x + Vector3.forward * colliderSize.z / 2;
        rightAxis.position = minCornerPos + Vector3.right * colliderSize.x / 2;


        upAxis.rotation = Quaternion.Euler(0, -90, -90);
        forwardAxis.rotation = Quaternion.Euler(90, 0, 90);
        rightAxis.rotation = Quaternion.Euler(90, 0, 0);

        //omg just invent something better, bc it is not stable and depends on texture dimensions
        upAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.y * 2, 1f);
        forwardAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.z * 2, 1f);
        rightAxis.GetComponentInChildren<SpriteRenderer>().size = new Vector2(colliderSize.x * 2, 1f);

        upAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.y * 100)} cm";
        forwardAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.z * 100)} cm";
        rightAxis.GetComponentInChildren<TextMeshPro>().text = $"{Round1Digit(colliderSize.x * 100)} cm";

        upAxis.SetParent(rulerRoot);
        rightAxis.SetParent(rulerRoot);
        forwardAxis.SetParent(rulerRoot);



        rulerRoot.gameObject.SetActive(RulerEnabled);
    }


    private float Round1Digit(float value)
    {
        return Mathf.Round(value * 10) / 10;
    }
}
