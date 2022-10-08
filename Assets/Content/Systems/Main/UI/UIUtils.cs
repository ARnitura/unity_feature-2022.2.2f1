using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIUtils
{

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsTouchOverUIObject()
    {

#if UNITY_EDITOR
        if (LeanTouch.Fingers.Count == 0)
            return false;
#else
        if(Input.touchCount == 0)
            return false;
#endif
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#if UNITY_EDITOR
        eventDataCurrentPosition.position = new Vector2(LeanTouch.Fingers[0].ScreenPosition.x, LeanTouch.Fingers[0].ScreenPosition.y);
#else
        eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
#endif
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static IEnumerator fadeGroup(CanvasGroup cg, float alpha, float time)
    {
        float t = 0;
        float startAlpha = cg.alpha;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / time;
            cg.alpha = Mathf.SmoothStep(startAlpha, alpha, t);
            yield return null;
        }
    }
    public static IEnumerator enableGroup(CanvasGroup cg, bool enable, float time)
    {
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float t = 0;
        float startAlpha = cg.alpha;
        float destination = enable ? 1 : 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / time;
            cg.alpha = Mathf.SmoothStep(startAlpha, destination, t);
            yield return null;
        }

        cg.interactable = enable;
        cg.blocksRaycasts = enable;
    }
    public static IEnumerator LerpImageColor(float duration, Image image, Color from, Color to)
    {
        float t = 0;
        while (t < 1)
        {
            image.color = Color.Lerp(from, to, t);

            t += Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }
    }
}
