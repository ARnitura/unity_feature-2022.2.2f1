using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ScanManager : MonoBehaviour
{
    [SerializeField]
    private ARPlaneManager planeManager;

    [SerializeField]
    private Image raycastBlocker;

    [SerializeField]
    private CanvasGroup helpStartGroup;
    [SerializeField]
    private CanvasGroup helpEndGroup;

    [SerializeField]
    private Slider scanSlider;

    [SerializeField]
    private Button scanEndButton;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField]
    private TextMeshProUGUI debugText;
#endif

    [Header("Android")]
    [SerializeField]
    private int androidTargetUpdatesCount = 10;
    [SerializeField]
    private int androidTargetPlanesCount = 1;

    [Header("IOS")]
    [SerializeField]
    private int iosTargetUpdatesCount = 20;
    [SerializeField]
    private int iosTargetPlanesCount = 1;

    //[SerializeField]
    private int targetUpdatesCount = 3;
    //[SerializeField]
    private int targetPlanesCount = 3;

    private int currentPlanesCount = 0;
    private int currentUpdatesCount = 0;


    public bool ScanComplete { get; private set; } = false;
    private bool scanCompleteInternal = false;
    private List<ARPlane> addedPlanes = new List<ARPlane>();

    // Start is called before the first frame update
    private void Start()
    {
        scanEndButton.onClick.AddListener(() => { EndButton().Forget(); });
        GlobalState.StateChanged += GlobalState_StateChanged;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        if (obj == GlobalState.State.Scan)
        {
            helpStartGroup.gameObject.SetActive(true);
            helpEndGroup.gameObject.SetActive(true);
            StartScan();
        }
        else
        {
            try
            {
                planeManager.planesChanged -= PlaneManager_planesChanged;
            }
            catch
            {

            }

            foreach (var item in addedPlanes)
                item.boundaryChanged -= Item_boundaryChanged;

            addedPlanes.Clear();


            helpStartGroup.gameObject.SetActive(false);
            helpEndGroup.gameObject.SetActive(false);

            helpEndGroup.blocksRaycasts = false;
            helpStartGroup.blocksRaycasts = false;

            helpEndGroup.alpha = 0;
            helpStartGroup.alpha = 0;
            scanSlider.value = 0;
            currentPlanesCount = 0;
            currentUpdatesCount = 0;
            raycastBlocker.enabled = false;
            planeManager.enabled = false;
        }
    }

    public void StartScan()
    {


        planeManager.planesChanged += PlaneManager_planesChanged;

        helpEndGroup.blocksRaycasts = false;
        helpStartGroup.blocksRaycasts = true;
        helpEndGroup.alpha = 0;
        helpStartGroup.LeanAlpha(1, 0.25f).setEaseInOutExpo();
        scanSlider.value = 0;

        ScanComplete = false;
        scanCompleteInternal = false;
        raycastBlocker.enabled = true;
        scanEndButton.enabled = false;

        planeManager.enabled = true;
#if !UNITY_EDITOR
        planeManager.subsystem.Start();
#endif

        currentPlanesCount = 0;
        currentUpdatesCount = 0;

#if UNITY_IOS
        targetUpdatesCount = iosTargetUpdatesCount;
        targetPlanesCount = iosTargetPlanesCount;
#elif UNITY_ANDROID
        targetUpdatesCount = androidTargetUpdatesCount;
        targetPlanesCount = androidTargetPlanesCount;
#endif

        Debug.Log("Scan started");

        scanSlider.maxValue = targetPlanesCount + targetUpdatesCount;
        //Debug.Log($"planeManager {(planeManager.enabled ? "online" : "offline") }");
        FindObjectOfType<PlacementIndicator>(true).gameObject.SetActive(false);
    }

    private async UniTask EndScan()
    {
        Debug.Log("Scan ended");

        foreach (var item in addedPlanes)
            item.boundaryChanged -= Item_boundaryChanged;

        addedPlanes.Clear();

        planeManager.planesChanged -= PlaneManager_planesChanged;

        planeManager.enabled = false;

        /*
#if !UNITY_EDITOR
        planeManager.subsystem.Stop();
#endif
        */

        scanCompleteInternal = true;
        scanEndButton.enabled = true;

        float duration = 0.5f;

        helpStartGroup.LeanAlpha(0, duration).setEaseInOutExpo();
        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        helpStartGroup.blocksRaycasts = false;

        helpEndGroup.LeanAlpha(1, duration).setEaseInOutExpo();
        await UniTask.Delay(TimeSpan.FromSeconds(duration));


        helpEndGroup.blocksRaycasts = true;

        //raycastBlocker.enabled = false;
    }

    private async UniTask EndButton()
    {
        helpEndGroup.blocksRaycasts = false;
        helpStartGroup.blocksRaycasts = false;
        scanEndButton.enabled = false;


        float duration = 0.5f;

        helpEndGroup.LeanAlpha(0, duration).setEaseInOutExpo();
        //await UniTask.Delay(TimeSpan.FromSeconds(duration));
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        raycastBlocker.enabled = false;
        FindObjectOfType<PlacementIndicator>(true).gameObject.SetActive(true);
        ScanComplete = true;
        GlobalState.SetState(GlobalState.State.ARObjectPlacement);

    }

    private void PlaneManager_planesChanged(ARPlanesChangedEventArgs obj)
    {

        if (scanCompleteInternal)
            return;


        if (currentPlanesCount >= targetPlanesCount && currentUpdatesCount >= targetUpdatesCount)
        {
            EndScan().Forget();
        }
        else
        {

            helpStartGroup.enabled = true;

            foreach (var item in obj.added)
            {
                addedPlanes.Add(item);
                item.boundaryChanged += Item_boundaryChanged;
            }
            currentPlanesCount += obj.added.Count;
            currentPlanesCount -= obj.removed.Count;


            if (currentPlanesCount > targetPlanesCount)
                currentPlanesCount = targetPlanesCount;

            if (currentPlanesCount < obj.updated.Count)
                currentPlanesCount = obj.updated.Count;

            Debug.Log($"Planes updated: target {targetPlanesCount}, actual {currentPlanesCount}");

            UpdateSlider().Forget();
        }

    }

    private void Item_boundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        if (scanCompleteInternal)
            return;

        currentUpdatesCount++;
        Debug.Log($"Plane boundary updated: target {targetUpdatesCount}, actual {currentUpdatesCount}");

        UpdateSlider().Forget();
    }

    private async UniTask UpdateSlider()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        debugText.text = $"Planes: {currentPlanesCount}/{targetPlanesCount}\n" +
                 $"Updates: {currentUpdatesCount}/{targetUpdatesCount}";
#endif
        float t = 0;
        float duration = 0.2f;
        float startValue = scanSlider.value;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            scanSlider.value = Mathf.SmoothStep(startValue, currentPlanesCount + currentUpdatesCount, t);
            await UniTask.Yield();
        }
    }


#if  UNITY_EDITOR
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartScan();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            EndScan().Forget();
        }



    }

#endif
}
