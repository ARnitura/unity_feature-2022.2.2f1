using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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




    public void WebLoadModel()
    {
        StartCoroutine(WebLoadModelCoroutine());
    }

    public void WebLoadAllTextures()
    {
        StartCoroutine(WebLoadAllTexturesCoroutine());
    }

    private IEnumerator WebLoadAllTexturesCoroutine()
    {

        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Leather_BaseColor.jpg", "Leather_BaseColor.jpg");
        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Leather_Normal.jpg", "Leather_Normal.jpg");
        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Leather_AO.jpg", "Leather_AO.jpg");
        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Leather_Specular.png", "Leather_Specular.png");

        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Wood_BaseColor.png", "Wood_BaseColor.png");
        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/textures/1/Wood_Normal.png", "Wood_Normal.png");

        string directory = $"{Application.persistentDataPath}/Files/";

        messages.LoadTexture(string.Join(", ", directory + "Leather_BaseColor.jpg", directory + "Leather_Normal.jpg", directory + "Leather_AO.jpg", directory + "Leather_Specular.png", directory + "Wood_BaseColor.png", directory + "Wood_Normal.png"));
    }


    private IEnumerator WebLoadModelCoroutine()
    {
        yield return WebLoadFile("https://arkreslo.ru/image/manufacturers/1/models/1.fbx", "model.fbx");

        messages.LoadModel($"{Application.persistentDataPath}/Files/model.fbx");
    }
    private IEnumerator WebLoadFile(string url, string fileName)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = new DownloadHandlerFile($"{Application.persistentDataPath}/Files/{fileName}");
            www.chunkedTransfer = false;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Texture myTexture = ((DownloadHandlerFile)www.downloadHandler).;
            }
        }
    }
#endif
}
