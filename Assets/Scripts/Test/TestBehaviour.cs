#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [SerializeField]
    FlutterMessages messages;

    string testFolderDir;
    private void Start()
    {
        
        testFolderDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, @"testFolder\");
        Debug.Log(Application.dataPath);
        Debug.Log(Directory.GetParent(Application.dataPath).FullName);
        Debug.Log(testFolderDir);
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
}

#endif
