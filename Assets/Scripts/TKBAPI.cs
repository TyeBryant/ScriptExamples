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

        public static Vector3 Intersect(ref Vector3 a, ref Vector3 b, float da, float db)
        {
            return a + (da / (da - db)) * (b - a);
        }
    }

    public class MeshAPI : MonoBehaviour
    {

        public class Poly
        {
            public List<Vector3> verts = new List<Vector3>();

            public Poly()
            {
            }

            public Poly(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                verts.Add(v1);
                verts.Add(v2);
                verts.Add(v3);
            }
        }

        public static void CreateMeshObject (List<Poly> polygonList, Vector3 position, Material mat)
        {
            GameObject newMeshObj = new GameObject("customMesh");
            newMeshObj.AddComponent(typeof(MeshFilter));
            newMeshObj.AddComponent(typeof(MeshRenderer));

            Mesh newMesh = newMeshObj.GetComponent<MeshFilter>().mesh;

            List<Vector3> newMeshVerts = new List<Vector3>();
            List<int> newMeshTris = new List<int>();

            for (int i = 0; i < polygonList.Count; ++i)
            {
                foreach (Vector3 vert in polygonList[i].verts)
                    newMeshVerts.Add(vert);
            }

            newMesh.SetVertices(newMeshVerts);

            int vertCount = newMesh.vertices.Length;
            for (int vertexIndex = 0; vertexIndex < vertCount * 3;)
            {
                newMeshTris.Add((0 + vertexIndex) % vertCount);
                newMeshTris.Add((1 + vertexIndex) % vertCount);
                newMeshTris.Add((2 + vertexIndex) % vertCount);

                newMeshTris.Add((2 + vertexIndex) % vertCount);
                newMeshTris.Add((1 + vertexIndex) % vertCount);
                newMeshTris.Add((0 + vertexIndex) % vertCount);

                vertexIndex += 3;
            }

            newMesh.SetTriangles(newMeshTris, 0);

            newMeshObj.GetComponent<MeshRenderer>().material = mat;

            newMeshObj.AddComponent(typeof(MeshCollider));
            newMeshObj.GetComponent<MeshCollider>().inflateMesh = true;
            newMeshObj.GetComponent<MeshCollider>().convex = true;
            //newMeshObj.AddComponent(typeof(Rigidbody));
            newMeshObj.transform.position = position;
            newMeshObj.transform.localScale *= 5;
        }

        public static void SutherlandHodgman (ref Vector3 n,  float d, ref Poly poly, ref List<Poly> frontOut, ref List<Poly> backOut)
        {
            Poly frontPoly = new Poly();
            Poly backPoly = new Poly();
            int s = poly.verts.Count;

            Vector3 a = poly.verts[s - 1];
			float da = Vector3.Dot(n, a) - d;

            for (int i = 0; i < s; ++i)
		    {
                Vector3 b = poly.verts[i];
                float db = Vector3.Dot(n, b) - d;
			 
				if (db > 0.000001f)
				{
					if (da < -0.000001f)
					{
                        Vector3 newI = VectorAPI.Intersect(ref b, ref a, db, da);
						frontPoly.verts.Add(newI);
						backPoly.verts.Add(newI);
					}

					frontPoly.verts.Add(b);
				}
				else if (db < -0.000001f)
				{;
                    if (da > 0.000001f)
                    {
                        Vector3 newI = VectorAPI.Intersect(ref a, ref b, da, db);
                        frontPoly.verts.Add(newI);
                        backPoly.verts.Add(newI);
                    }
                    else if (da < 0.000001f && da > -0.000001f)
                    {
                        backPoly.verts.Add(a);
                    }
				 
					backPoly.verts.Add(b );
				}
				else
				{
					frontPoly.verts.Add(b );

                    if (da < 0.000001f && da > -0.000001f)
                    {
                        backPoly.verts.Add(b);
                    }
				}
			 
				a = b;
				da = db;
			}
		 
			if (frontPoly.verts.Count > 0)
				frontOut.Add(frontPoly);
			if (backPoly.verts.Count > 0)
                backOut.Add(backPoly);
        }

        public static void SliceAllPolygons(Vector3 n, float d, List<Poly> allIn, ref List<Poly> frontOut, ref List<Poly> backOut)
        {
            for (int i = 0; i < allIn.Count; ++i)
            {
                Poly newPoly = allIn[i];
                SutherlandHodgman(ref n, d, ref newPoly, ref frontOut, ref backOut);
            }
        }
    }
}
