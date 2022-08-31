using System.IO;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [SerializeField]
    private FlutterMessagesReciever messages;
    private string testFolderDir;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private void Start()
    {

        testFolderDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, @"testFolder\");
        //Debug.Log(Application.dataPath);
        //Debug.Log(Directory.GetParent(Application.dataPath).FullName);
        //Debug.Log(testFolderDir);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
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

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //clear scene
            Debug.Log($"Emulating <close AR> call from flutter");
            messages.ClearAR();

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //clear scene
            Debug.Log($"Emulating <Start AR> call from flutter");
            messages.StartAR();

        }
    }
#endif
}
