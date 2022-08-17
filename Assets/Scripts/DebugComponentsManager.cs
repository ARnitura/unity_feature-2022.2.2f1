using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugComponentsManager : MonoBehaviour
{
    public GameObject[] debugObjects;
    public MonoBehaviour[] debugComponents;
    void Start()
    {

#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
        for (int i = 0; i < debugComponents.Length; i++)
        {
            Destroy(debugComponents[i]);
        }
        for (int i = 0; i < debugObjects.Length; i++)
        {
            Destroy(debugObjects[i]);
        }
#endif
    }
}
