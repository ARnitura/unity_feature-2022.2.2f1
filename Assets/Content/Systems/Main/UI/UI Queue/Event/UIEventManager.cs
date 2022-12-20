using System;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class UIEventManager : MonoBehaviour
{
    [SerializeField]
    private bool active;
    [SerializeField]
    private Transform queueV, queueA;
    private static UIEventManager instance;

    [SerializeField]
    private UIEvent visualElementPrefab;
    [SerializeField]
    private RectTransform referenceElementPrefab;
    private static readonly System.Random random = new System.Random();

    private enum LogDetails { NoStacktrace, FullStacktrace }

    [SerializeField]
    private LogDetails logDetails = LogDetails.FullStacktrace;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        Application.logMessageReceived += LogCallback;
    }

    private void OnDestroy()
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
        if (!instance)
            return;

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
        if (!instance.active)
            return;

        AddEvent($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{condition}</color>\n{stackTrace}", duration, 35);
    }
}
