using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKBAPI;

public class TrueClothGen : MonoBehaviour {

    [SerializeField]
    int width;

    private MeshFilter meshFilter;

    private GameObject[] verts;

    // Use this for initialization
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.MarkDynamic();
        ClothAPI.GenerateVerticies(width, ref verts, transform);
    }

    // Update is called once per frame
    void Update()
    {
        ClothAPI.ClothMath(width, 1f, 0.1f, 0.3f, ref verts, ref meshFilter);
        GenerateNormals();
        GenerateTriangles();
    }

    void GenerateTriangles()
    {
        int[] triangles = new int[(width - 1) * (width - 1) * 6 * 2];

        int triangleIndex = 0;

        for (int x = 0; x < width - 1; ++x)
        {
            for (int z = 0; z < width - 1; ++z)
            {
                int vertexIndex = x * width + z;

                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + width;
                triangles[triangleIndex + 2] = vertexIndex + width + 1;

                triangles[triangleIndex + 3] = vertexIndex;
                triangles[triangleIndex + 4] = vertexIndex + width + 1;
                triangles[triangleIndex + 5] = vertexIndex + 1;

                triangleIndex += 6;

                triangles[triangleIndex + 5] = vertexIndex;
                triangles[triangleIndex + 4] = vertexIndex + width;
                triangles[triangleIndex + 3] = vertexIndex + width + 1;

                triangles[triangleIndex + 2] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + width + 1;
                triangles[triangleIndex + 0] = vertexIndex + 1;

                triangleIndex += 6;
            }
        }
        meshFilter.mesh.triangles = triangles;
    }

    void GenerateNormals()
    {
        List<Vector3> normals = new List<Vector3>();

        foreach (Vector3 vertex in meshFilter.mesh.vertices)
        {
            normals.Add(Vector3.up);
        }

        meshFilter.mesh.SetNormals(normals);
    }
}
