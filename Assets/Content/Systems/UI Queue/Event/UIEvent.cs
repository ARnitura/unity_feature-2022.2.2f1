using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEvent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textField;
    [SerializeField]
    private Image textContainer;

    private Vector3 curVelocity = Vector3.zero;
    //private float offset = 0;
    private RectTransform referenceElement;


    private const float openTime = 0.1f;
    private const float closeTime = 0.2f;
    private float duration;


    public void Init(RectTransform a, string message, float duration)
    {
        referenceElement = a;
        textField.text = message;

        this.duration = duration;

        Invoke("Setup", 0.05f);
    }

    public void SetFontSize(float fontSize)
    {
        textField.fontSize = fontSize;
    }

    // Update is called once per frame
    private void Setup()
    {
        RectTransform containerRect = textContainer.GetComponent<RectTransform>();
        RectTransform textRect = textField.GetComponent<RectTransform>();

        //img height = text height
        containerRect.sizeDelta = new Vector2(0, textRect.rect.height);// = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().rect.height + offset;
        referenceElement.GetComponent<LayoutElement>().preferredHeight = containerRect.rect.height;
        StartCoroutine(Animate());
    }

    private void LateUpdate()
    {
        //Debug.Log("!!");
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, referenceElement.localPosition, ref curVelocity, Time.deltaTime * 10);
    }

    private IEnumerator Animate()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / openTime;
            yield return null;
            textContainer.fillAmount = Mathf.SmoothStep(0, 1, t);
        }

        yield return new WaitForSeconds(duration);

        t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / closeTime;
            yield return null;
            textContainer.fillAmount = Mathf.SmoothStep(0, 1, 1 - t);
        }

        Destroy(referenceElement.gameObject);
        Destroy(gameObject);
    }
}
