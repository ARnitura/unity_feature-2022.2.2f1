using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ScanManager : MonoBehaviour
{
    public ARPlaneManager planeManager;

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


    [SerializeField]
    private int targetUpdatesCount = 3;
    private int currentUpdatesCount = 0;


    public bool ScanComplete { get; private set; } = false;
    public bool scanCompleteInternal = false;


    // Start is called before the first frame update
    private void Start()
    {
        // ARSession.stateChanged += ARSession_stateChanged;
        planeManager.planesChanged += PlaneManager_planesChanged;
        scanSlider.maxValue = targetUpdatesCount;

        helpEndGroup.blocksRaycasts = false;
        helpStartGroup.blocksRaycasts = false;

        helpEndGroup.alpha = 0;
        helpStartGroup.alpha = 0;
        scanSlider.value = 0;
        currentUpdatesCount = 0;
        ScanComplete = false;
        raycastBlocker.enabled = false;




        scanEndButton.onClick.AddListener(() => { EndButton().Forget(); });

        // Debug.Log("on messages start");
    }


    public void StartScan()
    {
        helpEndGroup.blocksRaycasts = false;
        helpStartGroup.blocksRaycasts = true;
        helpEndGroup.alpha = 0;
        helpStartGroup.LeanAlpha(1, 0.25f).setEaseInOutExpo();
        scanSlider.value = 0;
        currentUpdatesCount = 0;
        ScanComplete = false;
        scanCompleteInternal = false;
        raycastBlocker.enabled = true;
        scanEndButton.enabled = false;

        //Debug.Log($"planeManager {(planeManager.enabled ? "online" : "offline") }");
        FindObjectOfType<PlacementIndicator>(true).gameObject.SetActive(false);
    }

    private async UniTask EndScan()
    {
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

    }

    private void PlaneManager_planesChanged(ARPlanesChangedEventArgs obj)
    {
        // Debug.Log("planes updated");
        if (scanCompleteInternal)
            return;

        if (currentUpdatesCount >= targetUpdatesCount)
        {
            EndScan().Forget();
        }
        else
        {

            helpStartGroup.enabled = true;

            currentUpdatesCount++;

            UpdateSlider().Forget();
        }

    }

    private async UniTask UpdateSlider()
    {
        float t = 0;
        float duration = 0.2f;
        float startValue = scanSlider.value;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            scanSlider.value = Mathf.SmoothStep(startValue, currentUpdatesCount, t);
            await UniTask.Yield();
        }
    }



    private void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartScan();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            EndScan().Forget();
        }
#endif
    }
}