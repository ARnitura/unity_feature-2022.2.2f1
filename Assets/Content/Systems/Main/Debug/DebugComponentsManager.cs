using UnityEngine;

public class DebugComponentsManager : MonoBehaviour
{
    public GameObject[] debugObjects;
    public MonoBehaviour[] debugComponents;

    private void Start()
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

#if UNITY_EDITOR
        for (int i = 0; i < debugComponents.Length; i++)
        {
            debugComponents[i].enabled = true;

        }
        for (int i = 0; i < debugObjects.Length; i++)
        {
            debugObjects[i].SetActive(true);
        }
#endif
    }
}
