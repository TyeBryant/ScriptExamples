using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GenerateVerticies();
    }

    // Update is called once per frame
    void Update()
    {
        ClothMath();
    }

    void GenerateVerticies()
    {
        List<GameObject> verticies = new List<GameObject>();

        for (int x = -width / 2; x < width / 2; ++x)
        {
            for (int z = -width / 2; z < width / 2; ++z)
            {
                GameObject newObj = Instantiate(new GameObject("cloth vert"), transform);
                newObj.transform.localPosition = new Vector3(x, 0, z);
                newObj.AddComponent<BoxCollider>();
                newObj.AddComponent<Rigidbody>();
                verticies.Add(newObj);
            }
        }

        verts = verticies.ToArray();
        //mesh.SetVertices(verticies);
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

    void ClothMath()
    {
        int vertIndex = 0;

        for (int x = 0; x < width - 1; ++x)
        {
            for (int z = 0; z < width - 1; ++z)
            {
                Vector3 temp1 = verts[vertIndex].transform.position;
                Vector3 temp2 = verts[vertIndex + width + 1].transform.position;
                Vector3 temp3 = verts[vertIndex + 1].transform.position;

                if (vertIndex % 10 == 0)
                {
                    constrain_3d_max_dist(ref temp1, ref temp2, 1f, 0.1f, 0.3f);
                    constrain_3d_max_dist(ref temp1, ref temp3, 1f, 0.1f, 0.3f);
                }
                else
                {
                    constrain_3d_max_dist(ref temp1, ref temp3, 10f, 0.1f, 0.3f);
                }

                verts[vertIndex].transform.position = temp1;
                verts[vertIndex + width + 1].transform.position = temp2;
                verts[vertIndex + 1].transform.position = temp3;

                ++vertIndex;
            }
        }

        List<Vector3> vertPoints = new List<Vector3>();

        foreach (var vert in verts)
        {
            vertPoints.Add(vert.transform.localPosition);
        }

        meshFilter.mesh.SetVertices(vertPoints);
        meshFilter.mesh.bounds.Expand(Mathf.Infinity);
        GenerateNormals();
        GenerateTriangles();
    }

    void constrain_3d_max_dist(ref Vector3 pos1, ref Vector3 pos2, float desired_dist, float compensate1, float compensate2)

    {
        Vector3 delta = pos2 - pos1;

        float deltaLength = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

        if (deltaLength > desired_dist)
        {

            float diff = (deltaLength - desired_dist) / deltaLength;

            pos1 += delta * compensate1 * diff;

            pos2 -= delta * compensate2 * diff;
        }

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
