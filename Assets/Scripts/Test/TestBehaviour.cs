using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [SerializeField]
    FlutterMessages messages;

    [SerializeField]
    TextMeshProUGUI fpsText; 

    string testFolderDir;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void Start()
    {
        
        testFolderDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, @"testFolder\");
        //Debug.Log(Application.dataPath);
        //Debug.Log(Directory.GetParent(Application.dataPath).FullName);
        //Debug.Log(testFolderDir);
        StartCoroutine(fpsOutput());
    }

    IEnumerator fpsOutput()
    {
        while (true)
        {

            fpsText.text = $"{(int)(1f / Time.unscaledDeltaTime)} fps";
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Q))
        {
            //load model
            Debug.Log($"Emulating <load model> call from flutter");
            messages.LoadModel(testFolderDir + "1.fbx");

            
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //load texture
            Debug.Log($"Emulating <load texture> call from flutter");
            string fullpath = string.Join(", ",
                testFolderDir + @"tex\Leather_BaseColor.jpg",
                testFolderDir + @"tex\Leather_Normal.jpg",
                testFolderDir + @"tex\Leather_AO.jpg",
                testFolderDir + @"tex\Wood_BaseColor.jpg",
                testFolderDir + @"tex\Wood_Normal.jpg"
                );

            messages.LoadTexture(fullpath);
           // messages.LoadTexture(testFolderDir + @"tex\Leather_BaseColor.jpg");
            //messages.LoadTexture(testFolderDir + @"tex\Leather_Normal.jpg");
           // messages.LoadTexture(testFolderDir + @"tex\Leather_AO.jpg");

           // messages.LoadTexture(testFolderDir + @"tex\Wood_BaseColor.jpg");
           // messages.LoadTexture(testFolderDir + @"tex\Wood_Normal.jpg");
           // messages.LoadTexture(testFolderDir + @"tex\Leather_AO.jpg");
            //messages.LoadTexture(testFolderDir + @"tex\Leather_Rough.jpg");

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //clear scene
            Debug.Log($"Emulating <close AR> call from flutter");
            messages.ClearAR();
            
        }
    }
#endif
}
