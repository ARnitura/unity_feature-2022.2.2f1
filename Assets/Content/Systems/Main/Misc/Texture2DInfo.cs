using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Texture2DInfo : IDisposable
{
    public enum TextureType { Albedo, Normal, Specular, AmbientOcclusion, SharedNormal, Invalid }


    public string Path { get; private set; }
    public string MaterialName { get; private set; }
    public Texture2D Texture { get; private set; }
    public TextureType Type { get; private set; }
    public bool WasApplied { get; private set; }

    public Texture2DInfo(string path)
    {
        Path = path;
        Texture = new Texture2D(2, 2);
        WasApplied = false;

        string[] nameParts = System.IO.Path.GetFileNameWithoutExtension(path).Split('_');
        StringBuilder matNameBuilder = new StringBuilder();


        for (int i = 0; i < nameParts.Length - 1; i++)
        {
            if (i != 0)
                matNameBuilder.AppendJoin('_', nameParts[i]);
            else
                matNameBuilder.Append(nameParts[i]);
        }


        MaterialName = matNameBuilder.ToString().Trim().ToLower();

        string typeName = nameParts[nameParts.Length - 1].ToLower();
        typeName.Trim();

#if true || UNITY_EDITOR
        if (typeName.Any(char.IsDigit))
            Debug.LogWarning($"Texture type contains digits! This is not allowed!\nTexture: {System.IO.Path.GetFileName(Path)}");
#endif
        typeName = typeName.RemoveDigits();

        Type = TextureType.Invalid;

        switch (typeName)
        {
            case "basecolor":
            case "color":
            case "albedo":
            case "diffuse":
            case "dif":
                Type = TextureType.Albedo;
                Texture = LoadTextureData(path);
                break;

            case "normal":
            case "nor":
            case "normalmap":
            case "nrm":
                if (MaterialName == string.Empty)
                    Type = TextureType.SharedNormal;
                else
                    Type = TextureType.Normal;
                Texture = LoadNormalData(path);
                break;

            case "specular":
            case "spec":
                Type = TextureType.Specular;
                Texture = LoadTextureData(path);
                break;

            case "ao":
            case "ambient":
            case "ambientocclusion":
                Type = TextureType.AmbientOcclusion;
                Texture = LoadTextureData(path);
                break;
        }

#if true || UNITY_EDITOR
        if (Type == TextureType.Invalid)
        {

            Debug.LogError($"Couldn't parse texture type\nMaterial<{MaterialName}>\nType<{typeName}>\nat {path}");
        }
#endif
    }

    public bool TryApplyToMaterial(Material mat)
    {
        string trimmedMatName = mat.name.ToLower();
        if (trimmedMatName.Contains("(instance)"))
            trimmedMatName = trimmedMatName.Replace("(instance)", "");

        trimmedMatName = trimmedMatName.Trim();


        if (Type != TextureType.AmbientOcclusion)
            if (!trimmedMatName.Contains(MaterialName) && !MaterialName.Contains(trimmedMatName))
            {
#if true || UNITY_EDITOR
                // Debug.LogError($"Refused texture {ToString()} on material <{trimmedMatName}>");
#endif
                return false;
            }

#if true || UNITY_EDITOR
        //Debug.LogWarning($"Connected texture {ToString()} on material <{mat.name.ToLower()}>");
#endif

        switch (Type)
        {
            case TextureType.Albedo:
                return ApplyToMaterial(mat, "_MainTex");
            case TextureType.Normal:
                return ApplyToMaterial(mat, "_BumpMap");


            case TextureType.SharedNormal:
                return ApplyToMaterial(mat, "_BumpMapShared");
            case TextureType.Specular:
                return ApplyToMaterial(mat, "_SpecGlossMap");
            case TextureType.AmbientOcclusion:
                return ApplyToMaterial(mat, "_OcclusionMap");
            default:
                return false;
        }
    }

    private bool ApplyToMaterial(Material mat, string texName)
    {
        if (mat.HasProperty(texName))
            mat.SetTexture(texName, Texture);
        else
        {
            Debug.LogError($"Failed to apply texture: MaterialName: <{MaterialName}> Type: {Type} - shader property {texName} doesn't exist");
            //Debug.LogError($"Failed to apply texture: MaterialName: <{MaterialName}> Type: {Type} - shader property {texName} doesn't exist");
            return false;
        }

        WasApplied = true;
        return true;
    }

    public override string ToString()
    {
        return $"MaterialName: <{MaterialName}> Type: {Type}";
    }

    public static Texture2D LoadTextureData(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);

            if (!tex.LoadImage(fileData)) //..this will auto-resize the texture dimensions.
#if true || UNITY_EDITOR
            {
                Debug.LogError($"Failed to load texture from path: {filePath} - invalid format / corrupted file");
                //Debug.LogError($"Failed to load texture from path: {filePath} - invalid format");
            }
#endif
        }
        else
        {
            Debug.LogError($"Failed to load texture from path: {filePath} - file doesn't exist");
        }
        return tex;
    }

    public static Texture2D LoadNormalData(string filePath)
    {
        Texture2D loadedNormalMap = LoadTextureData(filePath);

        // note format is now copied from the loaded texture
        Texture2D convertedNormalMap = new Texture2D(loadedNormalMap.width, loadedNormalMap.height, loadedNormalMap.format, true, true);

        // the documentation on this function says it's GPU side only, but this is a lie
        // this is roughly equivalent to:
        // convertedNormalMap.SetPixels32(loadedNormalMap.GetPixels32(0), 0);
        // if both textures are readable on the CPU, which they will be in this case
        Graphics.CopyTexture(loadedNormalMap, convertedNormalMap);
        convertedNormalMap.Apply();

        UnityEngine.Object.Destroy(loadedNormalMap);

        return convertedNormalMap;
    }

    public static List<Texture2DInfo> GetTexturesFromCombinedPath(string allPath)
    {
        List<Texture2DInfo> result = new List<Texture2DInfo>();
        string[] maps = allPath.Split(", ");

        foreach (string texturePath in maps)
            result.Add(new Texture2DInfo(texturePath));

        return result;
    }
    public static List<Texture2DInfo> GetTexturesFromPaths(List<string> paths)
    {
        List<Texture2DInfo> result = new List<Texture2DInfo>();

        foreach (string texturePath in paths)
            result.Add(new Texture2DInfo(texturePath));

        return result;
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(Texture);
    }
}
