using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEventManager : MonoBehaviour
{
    [SerializeField]
    private Transform queueV, queueA;
    private static UIEventManager instance;

    [SerializeField]
    private UIEvent visualElementPrefab;
    [SerializeField]
    private RectTransform referenceElementPrefab;
    private static System.Random random = new System.Random();

    private enum LogDetails { NoStacktrace, FullStacktrace }

    [SerializeField]
    LogDetails logDetails = LogDetails.FullStacktrace;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            string a = "<color=\"red\">Super Important message</color> " + RandomString(UnityEngine.Random.Range(0, 55));
            AddEvent(a, 4);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            string a = "<color=\"yellow\">Important message</color> " + RandomString(UnityEngine.Random.Range(0, 55));
            AddEvent(a, 4);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            string a = "<color=\"green\">Message</color> " + RandomString(UnityEngine.Random.Range(0, 55));
            AddEvent(a, 4);
        }
    }
#endif

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    public static void AddEvent(string message, float duration, float fontSize = 30)
    {
        RectTransform a = Instantiate(instance.referenceElementPrefab, instance.queueA);
        UIEvent v = Instantiate(instance.visualElementPrefab, instance.queueV);

        v.Init(a, message, duration);
        v.SetFontSize(fontSize);
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        stackTrace = logDetails == LogDetails.FullStacktrace ? stackTrace : "";
        switch (type)
        {
            case LogType.Error:
                LogInternal(condition, stackTrace, 10, Color.red);
                break;
            case LogType.Assert:
                LogInternal(condition, stackTrace, 10, Color.white);
                break;
            case LogType.Warning:
                LogInternal(condition, stackTrace, 10, Color.yellow);
                break;
            case LogType.Log:
                LogInternal(condition, stackTrace, 10, Color.white);
                break;
            case LogType.Exception:
                LogInternal(condition, stackTrace, 10, Color.red);
                break;
        }

    }
    private static void LogInternal(string condition, string stackTrace, float duration, Color color)
    {
        AddEvent($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{condition}</color>\n{stackTrace}", duration, 35);
    }
    public static void LogException(Exception e, float duration)
    {
        AddEvent($"<color=red>{e.GetType().Name}</color>\n{e.StackTrace}\n{e.Data}", duration, 35);
    }
}
