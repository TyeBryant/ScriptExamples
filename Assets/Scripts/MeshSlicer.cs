using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshSlicer : MonoBehaviour {

    private List<Vector3> vertsToSplit = new List<Vector3>();
    private List<Vector3> meshVerts = new List<Vector3>();
    public List<Vector3> originalVerticies;
    public List<Vector3> modifiedVerticies;

    private Vector3 biggestX,
        smallestX,
        biggestY,
        smallestY,
        biggestZ,
        smallestZ;

    private void OnCollisionStay(Collision col)
    {
        if (!col.collider.CompareTag("Player"))
        {
            Debug.Log(col.collider.GetComponent<MeshFilter>().mesh.vertexCount);

            foreach (ContactPoint contPoint in col.contacts)
            {
                //Track the verts
                vertsToSplit.Add(contPoint.point);

                if (biggestX == Vector3.zero)
                    biggestX = contPoint.point;
                else if (biggestX.x < contPoint.point.x)
                    biggestX = contPoint.point;

                if (smallestX == Vector3.zero)
                    smallestX = contPoint.point;
                else if (smallestX.x > contPoint.point.x)
                    smallestX = contPoint.point;

                /*
                if (biggestY == Vector3.zero)
                    biggestY = contPoint.point;
                else if (biggestY.y < contPoint.point.y)
                    biggestY = contPoint.point;

                if (smallestY == Vector3.zero)
                    smallestY = contPoint.point;
                else if (smallestY.y > contPoint.point.y)
                    smallestY = contPoint.point;
                */

                if (biggestZ == Vector3.zero)
                    biggestZ = contPoint.point;
                else if (biggestZ.z < contPoint.point.z)
                    biggestZ = contPoint.point;

                if (smallestZ == Vector3.zero)
                    smallestZ = contPoint.point;
                else if (smallestZ.z > contPoint.point.z)
                    smallestZ = contPoint.point;
            }

            originalVerticies = col.collider.GetComponent<MeshFilter>().mesh.vertices.ToList<Vector3>();
            modifiedVerticies = col.collider.GetComponent<MeshFilter>().mesh.vertices.ToList<Vector3>();
            //meshVerts.RemoveAt(10);

            //List<Vector3> meshVerts = new List<Vector3>();

            foreach (Vector3 vert in vertsToSplit)
            {
                var worldPos4 = col.collider.transform.worldToLocalMatrix * vert;

                var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);

                for (int i = 0; i < modifiedVerticies.Count; ++i)
                {
                    var distance = (worldPos - modifiedVerticies[i]).magnitude;
                    if (distance < 1)
                    {
                        //var newVert = originalVerticies[i] + direction * maximumDepression * (distance / radius);
                        modifiedVerticies.RemoveAt(i);
                    }
                }
            }

            int vertCOunt = modifiedVerticies.Count / 3;

            int[] triangles = new int[(vertCOunt - 1) * (vertCOunt - 1) * 6];

            int triangleIndex = 0;
            for (int x = 0; x <  - 1; ++x)
            {
                for (int y = 0; y < vertCOunt - 1; ++y)
                {
                    int vertexIndex = x * vertCOunt + y;

                    triangles[triangleIndex + 0] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + vertCOunt;
                    triangles[triangleIndex + 2] = vertexIndex + vertCOunt + 1;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + vertCOunt + 1;
                    triangles[triangleIndex + 5] = vertexIndex + 1;

                    triangleIndex += 6;
                }
            }

            col.collider.GetComponent<MeshFilter>().mesh.triangles = triangles.Reverse().ToArray();
            col.collider.GetComponent<MeshFilter>().mesh.SetVertices(modifiedVerticies);
            Debug.Log(col.collider.GetComponent<MeshFilter>().mesh.vertexCount);
        }
        //vertsToSplit.Clear();
    }
}
