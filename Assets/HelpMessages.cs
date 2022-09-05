using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class HelpMessages : MonoBehaviour
{
    public ARPlaneManager planeManager;

    [SerializeField]
    private CanvasGroup helpStartGroup;
    [SerializeField]
    private CanvasGroup helpEndGroup;

    [SerializeField]
    private Slider scanSlider;

    [SerializeField]
    private int targetUpdatesCount = 3;
    private int currentUpdatesCount = 0;
    private bool scanEnded = false;

    // Start is called before the first frame update
    private void Start()
    {
        // ARSession.stateChanged += ARSession_stateChanged;
        planeManager.planesChanged += PlaneManager_planesChanged;
        scanSlider.maxValue = targetUpdatesCount;

        ResetState();
    }

    private void PlaneManager_planesChanged(ARPlanesChangedEventArgs obj)
    {
        if (scanEnded)
            return;

        if (currentUpdatesCount >= targetUpdatesCount)
        {
            scanEnded = true;
            StartCoroutine(AnimateScanEnd());
        }
        else
        {
            helpStartGroup.enabled = true;

            currentUpdatesCount++;
            StopAllCoroutines();
            StartCoroutine(MoveScanSlider());
        }

    }

    public void ResetState()
    {
        currentUpdatesCount = 0;

        helpEndGroup.alpha = 0;
        helpStartGroup.alpha = 1;

        helpEndGroup.blocksRaycasts = false;
        helpStartGroup.blocksRaycasts = true;

        scanEnded = false;
    }

    private IEnumerator MoveScanSlider()
    {
        float t = 0;
        float duration = 0.2f;

        float startValue = scanSlider.value;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            scanSlider.value = Mathf.SmoothStep(startValue, currentUpdatesCount, t);
            yield return null;
        }


    }

    private IEnumerator AnimateScanEnd()
    {
        float t = 0;
        float duration = 0.5f;

        //Color startColor = helpStartGroup.color;
        //Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            helpStartGroup.alpha = Mathf.SmoothStep(1, 0, t);
            yield return null;
        }
        helpStartGroup.blocksRaycasts = false;
        helpEndGroup.blocksRaycasts = true;
        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            helpEndGroup.alpha = Mathf.SmoothStep(0, 1, t);// Color.Lerp(endColor, startColor, t);
            yield return null;
        }
    }

    private IEnumerator AnimateHelpEnd()
    {
        float t = 0;
        float duration = 0.5f;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            helpEndGroup.alpha = Mathf.SmoothStep(1, 0, t);// Color.Lerp(endColor, startColor, t);
            yield return null;
        }
        helpEndGroup.blocksRaycasts = false;
    }
    public void HelpEndButton()
    {
        StartCoroutine(AnimateHelpEnd());
    }
}
