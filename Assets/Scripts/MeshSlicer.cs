using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TKBAPI;

public class MeshSlicer : MonoBehaviour {
    public Transform top,
        bottom;

    private Vector3 n1;
    private Vector3 n2;
    private Vector3 n3;
    private Vector3 n4;

    private void OnTriggerEnter(Collider col)
    {
        n1 = col.transform.InverseTransformVector(transform.position);
        n3 = col.transform.InverseTransformVector(top.position);
        n4 = col.transform.InverseTransformVector(bottom.position);
    }

    private void OnTriggerExit(Collider col)
    {
        n2 = col.transform.InverseTransformVector(transform.position);

        Mesh otherMesh = col.GetComponent<MeshFilter>().mesh;
        List<MeshAPI.Poly> meshPolys = new List<MeshAPI.Poly>();
        List<MeshAPI.Poly> frontOut = new List<MeshAPI.Poly>();
        List<MeshAPI.Poly> backOut = new List<MeshAPI.Poly>();

        for (int i = 0; i < otherMesh.triangles.Length;)
        {
            MeshAPI.Poly newPoly = new MeshAPI.Poly(otherMesh.vertices[otherMesh.triangles[i]], otherMesh.vertices[otherMesh.triangles[i+1]], otherMesh.vertices[otherMesh.triangles[i+2]]);
            meshPolys.Add(newPoly);
            i += 3;
        }

        Plane nPlane = new Plane(n3, n2, n4);

        Vector3 n = nPlane.normal;
        n.Normalize();

        float d = Vector3.Distance(n, n2);

        MeshAPI.SliceAllPolygons(n, d, meshPolys, ref frontOut, ref backOut);

        MeshAPI.CreateMeshObject(frontOut, col.transform.position - n * (col.transform.localScale.y / 2 + 0.1f), col.GetComponent<MeshRenderer>().material);
        MeshAPI.CreateMeshObject(backOut, col.transform.position - n * (col.transform.localScale.y / 2), col.GetComponent<MeshRenderer>().material);

        //Destroy(col.gameObject);

        Debug.Log("front: " + frontOut.Count);
        Debug.Log("back: " + backOut.Count);
        Debug.Log("n: " + n);
        Debug.Log("n1: " + n1);
        Debug.Log("n2: " + n2);
        Debug.Log("n3: " + n3);
        Debug.Log("n4: " + n4);
        Debug.Log("d: " + d);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(n3, n2);
        Gizmos.DrawLine(n2, n4);
        Gizmos.DrawLine(n4, n3);
        /*foreach (MeshAPI.Poly poly in frontOut)
            foreach (Vector3 vert in poly.verts)
                Gizmos.DrawSphere(vert, 0.01f);

        foreach (MeshAPI.Poly poly in backOut)
            foreach (Vector3 vert in poly.verts)
                Gizmos.DrawSphere(vert + Vector3.up, 0.01f);*/
    }
}