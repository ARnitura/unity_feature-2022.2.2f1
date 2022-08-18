using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture2DInfo
{
    public Texture2D Texture { get; private set; }
    public string Path { get; private set; }

    public Texture2DInfo(Texture2D tex, string path)
    {
        Texture = tex;
        Path = path;
    }
}
