using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPhys : MonoBehaviour {

    MeshFilter meshF;
    MeshCollider meshCol;

    int meshwidth;

    // Use this for initialization
    void Start () {
        meshF = GetComponent<MeshFilter>();
        meshF.mesh.MarkDynamic();
        meshCol = GetComponent<MeshCollider>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3[] verts = meshF.mesh.vertices;

        int vertIndex = 0;

        for (int x = 0; x < (int)meshF.mesh.bounds.extents.x * 2; ++x)
        {
            for (int y = 0; y < (int)meshF.mesh.bounds.extents.z * 2 + 1; ++y)
            {
                if (vertIndex > (int)meshF.mesh.bounds.extents.x * 2)
                {
                    verts[vertIndex] += Vector3.down * 9.8f * Time.deltaTime;
                    constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + (int)meshF.mesh.bounds.extents.x * 2 + 1], 1f, 0.05f, 0.2f);
                    if (verts[vertIndex].z == verts[vertIndex + 1].z)
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + 1], 1f, 0.05f, 0.2f);
                }
                else
                {
                    constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + (int)meshF.mesh.bounds.extents.x * 2 + 1], 1f, 0f, 0.5f);
                    if (verts[vertIndex].z == verts[vertIndex + 1].z)
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + 1], 1f, 0f, 0f);
                }
                ++vertIndex;
            }
        }

        meshF.mesh.vertices = verts;
        meshCol.sharedMesh = meshF.mesh;
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

    /*private void OnCollisionEnter(Collision col)
    {

        Vector3[] verts = meshF.mesh.vertices;

        int vertIndex = 0;

        for (int x = 0; x < (int)meshF.mesh.bounds.extents.x * 2; ++x)
        {
            for (int y = 0; y < (int)meshF.mesh.bounds.extents.z * 2; ++y)
            {
                foreach (var contPoint in col.contacts)
                {
                    if (vertIndex > (int)meshF.mesh.bounds.extents.x * 2)
                    {
                        verts[vertIndex] += Vector3.down * 9.8f * Time.deltaTime;
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + (int)meshF.mesh.bounds.extents.x * 2], 1f, 0.05f, 0.2f);
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + 1], 1f, 0.05f, 0.2f);
                    }
                    else if (Vector3.Distance(contPoint.point, verts[vertIndex]) < 0.1f)
                    {
                        Vector3 temp1 = contPoint.point;
                        constrain_3d_max_dist(ref verts[vertIndex], ref temp1, 1f, 0.25f, 0f);
                        constrain_3d_max_dist(ref verts[vertIndex], ref temp1, 1f, 0.25f, 0f);
                    }
                    else
                    {
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + (int)meshF.mesh.bounds.extents.x * 2], 0.1f, 0f, 0.5f);
                        constrain_3d_max_dist(ref verts[vertIndex], ref verts[vertIndex + 1], 0.1f, 0f, 0f);
                    }
                }
                ++vertIndex;
            }
        }

        meshF.mesh.vertices = verts;
        meshCol.sharedMesh = meshF.mesh;
    }*/
}
