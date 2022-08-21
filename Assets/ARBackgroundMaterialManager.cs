using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBackgroundMaterialManager : MonoBehaviour
{
    [SerializeField]
    Material androidMaterial, iosMaterial;

    void Start()
    {
        ARCameraBackground cameraBackground = GetComponent<ARCameraBackground>();
        cameraBackground.useCustomMaterial = true;
#if UNITY_ANDROID
        cameraBackground.customMaterial = androidMaterial;
#endif
#if UNITY_IOS
        cameraBackground.customMaterial = iosMaterial;
#endif
    }

}
