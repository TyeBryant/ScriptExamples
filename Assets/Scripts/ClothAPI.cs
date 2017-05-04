using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKBAPI
{
    public class ClothAPI : MonoBehaviour
    {

        public static void GenerateVerticies(int width, ref GameObject[] verts, Transform transform)
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
        }

        public static void ClothMath (int width, float maxVertDist, float vertShift1, float vertShift2, ref GameObject[] verts, ref MeshFilter meshFilter)
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
                        VectorAPI.constrain_3d_max_dist(ref temp1, ref temp2, maxVertDist, vertShift1, vertShift2);
                        VectorAPI.constrain_3d_max_dist(ref temp1, ref temp3, maxVertDist, vertShift1, vertShift2);
                    }
                    else
                    {
                        VectorAPI.constrain_3d_max_dist(ref temp1, ref temp3, maxVertDist * 10, vertShift1, vertShift2);
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
        }
    }

    public class VectorAPI : MonoBehaviour
    {
        public static void constrain_3d_max_dist(ref Vector3 pos1, ref Vector3 pos2, float desired_dist, float compensate1, float compensate2)
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
    }
}
