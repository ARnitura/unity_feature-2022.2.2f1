using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TestBehaviour : MonoBehaviour
{
    [SerializeField]
    private FlutterMessagesReciever messages;
    [SerializeField]
    private ARObjectLoader loader;
    private string testFolderDir;
    private string autoTestFolder;
    private int currentFolderIndex = 0;
    private int maxTestCount = 0;
    private int texFolderIndex = 0;

#if true || UNITY_EDITOR 
    private void Start()
    {
        testFolderDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, @"testFolder\");//Debug.Log(Application.dataPath);//Debug.Log(Directory.GetParent(Application.dataPath).FullName);//Debug.Log(testFolderDir);
        autoTestFolder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, @"autoTestFolder\");//Debug.Log(Application.dataPath);//Debug.Log(Directory.GetParent(Application.dataPath).FullName);//Debug.Log(testFolderDir);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //load model
            Debug.Log($"Emulating <load model> call from flutter");
            //messages.LoadModel(testFolderDir + "1.fbx");
            messages.LoadModel(testFolderDir + "lumix.fbx");
            //messages.LoadModel(testFolderDir + "SofaBoneAnimExample.fbx");



        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //load texture
            Debug.Log($"Emulating <load texture> call from flutter");
            /*
            string fullpath = string.Join(", ",
                testFolderDir + @"tex\Leather_BaseColor.jpg",
                testFolderDir + @"tex\Leather_Normal.jpg",
                testFolderDir + @"tex\Leather_AO.jpg",
                testFolderDir + @"tex\Wood_BaseColor.jpg",
                testFolderDir + @"tex\Wood_Normal.jpg"
                );
            */


            messages.LoadTexture(string.Join(", ", Directory.GetFiles(Path.Join(testFolderDir, "tex2"))));

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

        if (Input.GetKeyDown(KeyCode.T))
        {
            messages.ClearAR();
            messages.StartAR();
            string[] folders = Directory.GetDirectories(autoTestFolder);
            string texturesFolder = string.Empty;

            for (int i = 0; i < folders.Length; i++)
            {
                //Debug.Log(Path.GetFileName(folders[i]));
                if (Path.GetFileName(folders[i]).ToLower() == "textures")
                {
                    texturesFolder = folders[i];
                    maxTestCount = folders.Length - 1;
                    texFolderIndex = i;
                    break;
                }
            }

            if (texturesFolder == string.Empty)
                throw new IOException("No textures folder in autotest folder");


            if (currentFolderIndex == texFolderIndex)
                currentFolderIndex++;

            List<string> texturePaths = new List<string>();

            //get next model test folder
            string currentModelFolder = folders[currentFolderIndex];

            string[] files = Directory.GetFiles(currentModelFolder);
            //load model
            foreach (var item in files)
            {
                if (item.Contains(".fbx"))
                {
                    messages.LoadModel(item);
                }
                else if (item.Contains(".png") || item.Contains(".jpg"))
                {
                    texturePaths.Add(item);
                }
                else
                {
                    Debug.LogError($"Unknown file at {item}");
                }
            }

            foreach (var item in GetFiles(folders[texFolderIndex]))
            {
                if (item.Contains(".png") || item.Contains(".jpg"))
                {
                    texturePaths.Add(item);
                }
                else
                {
                    Debug.LogError($"Unknown file at {item}");
                }
            }

            //we can't deside at this moment which textures are correct, so my load mechanism will deside it
            loader.onModelLoaded = null;
            loader.onModelLoaded += () => { loader.LoadTextures(texturePaths); loader.ARObject.gameObject.SetActive(true); };

            currentFolderIndex++;
        }
    }

    private static IEnumerable<string> GetFiles(string path)
    {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(path);
        while (queue.Count > 0)
        {
            path = queue.Dequeue();
            try
            {
                foreach (string subDir in Directory.GetDirectories(path))
                {
                    queue.Enqueue(subDir);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    yield return files[i];
                }
            }
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
