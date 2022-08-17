using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;
using TriLibCore.General;

public class LoadModels : MonoBehaviour
{
    // Start is called before the first frame update
    private string testPath = "D:/android_developer/app_unity_to_fl/flutter_app1/unity/ar-kreslo/Assets/five";
    void Start()
    {
        loadModelPath();
    }

    public void loadModelPath()
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(true);
 
        // Loads the model from the "PATH_TO_MY_FILE.FBX" path
        AssetLoader.LoadModelFromFile(testPath, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnBeginLoad(bool anyModelSelected)
    {
 
    }
 
    // This event is called when the model loading progress changes.
    // You can use this event to update a loading progress-bar, for instance.
    // The "progress" value comes as a normalized float (goes from 0 to 1).
    // Platforms like UWP and WebGL don't call this method at this moment, since they don't use threads.
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
 
    }
 
    // This event is called when there is any critical error loading your model.
    // You can use this to show a message to the user.
    private void OnError(IContextualizedError contextualizedError)
    {
 
    }
 
    // This event is called when all model GameObjects and Meshes have been loaded.
    // There may still Materials and Textures processing at this stage.
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        // The root loaded GameObject is assigned to the "assetLoaderContext.RootGameObject" field.
        // If you want to make sure the GameObject will be visible only when all Materials and Textures have been loaded, you can disable it at this step.
        var myLoadedGameObject = assetLoaderContext.RootGameObject;
        myLoadedGameObject.SetActive(false);
    }
 
    // This event is called after OnLoad when all Materials and Textures have been loaded.
    // This event is also called after a critical loading error, so you can clean up any resource you want to.
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        // The root loaded GameObject is assigned to the "assetLoaderContext.RootGameObject" field.
        // You can make the GameObject visible again at this step if you prefer to.
        var myLoadedGameObject = assetLoaderContext.RootGameObject;
        myLoadedGameObject.SetActive(true);
        Debug.Log(assetLoaderContext.Filename);
    }
    
}
