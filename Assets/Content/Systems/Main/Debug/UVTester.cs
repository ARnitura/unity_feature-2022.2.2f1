using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTester : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        SkinnedMeshRenderer skin = GetComponent<SkinnedMeshRenderer>();

        if (meshFilter != null)
            PrintUVS(meshFilter.mesh);
        else if (skin != null)
            PrintUVS(skin.sharedMesh);
    }

    private void PrintUVS(Mesh mesh)
    {
        Debug.Log($"printing uv sets for mesh {mesh.name}");


        Debug.Log("Printing uv1");
        PrintUVSet(mesh.uv);


        Debug.Log("Printing uv2");
        PrintUVSet(mesh.uv2);


        Debug.Log("Printing uv3");
        PrintUVSet(mesh.uv3);


        Debug.Log("Printing uv4");
        PrintUVSet(mesh.uv4);


        Debug.Log("Printing uv5");
        PrintUVSet(mesh.uv5);


        Debug.Log("Printing uv6");
        PrintUVSet(mesh.uv6);


        Debug.Log("Printing uv7");
        PrintUVSet(mesh.uv7);


        Debug.Log("Printing uv8");
        PrintUVSet(mesh.uv8);

    }

    private void PrintUVSet(Vector2[] uvSet)
    {
        foreach (var item in uvSet)
        {
            Debug.Log(item);
        }
    }

}
