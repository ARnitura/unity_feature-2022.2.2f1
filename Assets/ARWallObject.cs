using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ARWallObject : MonoBehaviour
{

    private Mesh mesh;


    //debug fields
    [SerializeField]
    private Transform from;
    [SerializeField]
    private Transform to;

    public ARWallAnchor FromAnchor { get; private set; }
    public ARWallAnchor ToAnchor { get; private set; }


    private MeshCollider meshCollider;



#if UNITY_EDITOR
    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        //mesh = new Mesh { name = "Procedural Mesh" };
        //UpdateMesh();
        //AssignMesh();
    }


    private void Update()
    {
        //UpdateMesh();
        // AssignMesh();
    }
#endif


    internal void InitEmpty()
    {
        meshCollider = GetComponent<MeshCollider>();
        FromAnchor = ToAnchor = null;
        mesh = new Mesh { name = "Procedural Mesh" };
        AssignMesh();
    }

    internal void CreateMesh(ARWallAnchor to, ARWallAnchor from)
    {
        meshCollider = GetComponent<MeshCollider>();
        FromAnchor = from;
        ToAnchor = to;

        mesh = new Mesh { name = "Procedural Mesh" };
        this.from = from.transform;
        this.to = to.transform;


        UpdateMesh();
        AssignMesh();
    }

    //TODO: use low-level stream mesh API
    public void UpdateMesh()
    {
        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;

        const float wallHeight = 2.5f;
        const float wallThickness = 0.25f;

        float wallWidth = Vector3.Distance(fromPos, toPos);



        Vector3 upVector = Vector3.up * wallHeight;

        Vector3 leftBottom = fromPos;
        Vector3 leftTop = fromPos + upVector;

        Vector3 rightBottom = toPos;
        Vector3 rightTop = toPos + upVector;

        Vector3 forwardDir = Vector3.Cross(upVector, rightBottom - leftBottom).normalized;

        Vector3 forwardVector = forwardDir * wallThickness;

        mesh.vertices = new Vector3[] {
            //forward plane
            leftBottom + forwardVector/2, rightBottom+ forwardVector/2, leftTop+ forwardVector/2, rightTop + forwardVector / 2,
            //back plane
            rightBottom - forwardVector/2, leftBottom - forwardVector/2,  rightTop - forwardVector/2,  leftTop - forwardVector/2,


        };
        mesh.triangles = new int[] {
            //frw
            0, 2, 1,    1,2,3,
            //back
            4, 6, 5,     5,6,7,
            //left cap
            5,7,0,      0,7,2,

            //right cap
            1,3,4,      4,3,6,

            //top cap
            7,6,2,      2,6,3
        };

        mesh.normals = new Vector3[] {
            forwardDir, forwardDir, forwardDir, forwardDir,
            -forwardDir, -forwardDir, -forwardDir, -forwardDir

        };



        mesh.uv = new Vector2[] {
            Vector2.zero, Vector2.right * wallWidth, Vector2.up * wallHeight, new Vector2(1f*wallWidth, 1f*wallHeight),
            Vector2.zero, Vector2.right * wallWidth, Vector2.up * wallHeight, new Vector2(1f*wallWidth, 1f*wallHeight)
        };

        mesh.RecalculateBounds();

        AssignCollider();
    }

    private void AssignCollider()
    {
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    public void UpdateMeshFromPos(Vector3 from, Vector3 to)
    {
        Vector3 fromPos = from;
        Vector3 toPos = to;

        const float wallHeight = 2.5f;
        const float wallThickness = 0.25f;

        float wallWidth = Vector3.Distance(fromPos, toPos);



        Vector3 upVector = Vector3.up * wallHeight;

        Vector3 leftBottom = fromPos;
        Vector3 leftTop = fromPos + upVector;

        Vector3 rightBottom = toPos;
        Vector3 rightTop = toPos + upVector;

        Vector3 forwardDir = Vector3.Cross(upVector, rightBottom - leftBottom).normalized;

        Vector3 forwardVector = forwardDir * wallThickness;

        mesh.vertices = new Vector3[] {
            //forward plane
            leftBottom + forwardVector/2, rightBottom+ forwardVector/2, leftTop+ forwardVector/2, rightTop + forwardVector / 2,
            //back plane
            rightBottom - forwardVector/2, leftBottom - forwardVector/2,  rightTop - forwardVector/2,  leftTop - forwardVector/2,


        };
        mesh.triangles = new int[] {
            //frw
            0, 2, 1,    1,2,3,
            //back
            4, 6, 5,     5,6,7,
            //left cap
            5,7,0,      0,7,2,

            //right cap
            1,3,4,      4,3,6,

            //top cap
            7,6,2,      2,6,3
        };

        mesh.normals = new Vector3[] {
            forwardDir, forwardDir, forwardDir, forwardDir,
            -forwardDir, -forwardDir, -forwardDir, -forwardDir

        };



        mesh.uv = new Vector2[] {
            Vector2.zero, Vector2.right * wallWidth, Vector2.up * wallHeight, new Vector2(1f*wallWidth, 1f*wallHeight),
            Vector2.zero, Vector2.right * wallWidth, Vector2.up * wallHeight, new Vector2(1f*wallWidth, 1f*wallHeight)
        };

        mesh.RecalculateBounds();
        AssignCollider();
    }
    private void AssignMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
