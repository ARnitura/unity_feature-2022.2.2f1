using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Texture2DInfo
{
    public enum TextureType { Albedo, Normal, Specular, AmbientOcclusion, Invalid }


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
        MaterialName = nameParts[0];
        Type = TextureType.Invalid;

        switch (nameParts[1].ToLower())
        {
            case "basecolor":
                Type = TextureType.Albedo;
                Texture = LoadTextureData(path);
                break;
            case "normal":
                Type = TextureType.Normal;
                Texture = LoadNormalData(path);
                break;
            case "specular":
                Type = TextureType.Specular;
                Texture = LoadTextureData(path);
                break;
            case "ao":
                Type = TextureType.AmbientOcclusion;
                Texture = LoadTextureData(path);
                break;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (Type == TextureType.Invalid)
        {
            Debug.LogError($"Couldn't parse texture type\nMaterial<{MaterialName}>\nType<{nameParts[1]}>\nat {path}");
        }
#endif
    }

    public bool TryApplyToMaterial(Material mat)
    {
        if (!mat.name.Contains(MaterialName))
            return false;

        switch (Type)
        {
            case TextureType.Albedo:
                return ApplyToMaterial(mat, "_MainTex");
            case TextureType.Normal:
                return ApplyToMaterial(mat, "_BumpMap");
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
        mat.SetTexture(texName, Texture);
        WasApplied = true;
        return true;
    }

    public override string ToString()
    {
        return $"MaterialName: {MaterialName} Type: {Type}";
    }

    public static Texture2D LoadTextureData(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!tex.LoadImage(fileData)) //..this will auto-resize the texture dimensions.
            {
                Debug.LogError($"Failed to load texture from path: {filePath}");
            }
#endif
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

}