using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalStateDebugText : MonoBehaviour
{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void Awake()
    {
        GlobalState.StateChanged += GlobalState_StateChanged;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        GetComponent<TextMeshProUGUI>().text = $"Current mode: {obj}";
    }
#endif
}
