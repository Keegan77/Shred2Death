using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundTesting : MonoBehaviour
{
    private Collider collider;
    [SerializeField] private MeshFilter mesh;


    public Vector3[] verts;
    
    private void Start()
    {
        verts = mesh.sharedMesh.vertices;
        collider = GetComponent<MeshCollider>();
        //mesh.triangles = collider.GetComponent<MeshCollider>().sharedMesh.triangles;
    }

    private void Update()
    {
        //Debug.Log(collider.bounds);
    }
    
    float GetSqrMagFromEdge(Vector3 vertex1, Vector3 vertex2, Vector3 point)
    {
        float n = Vector3.Cross(point - vertex1, point - vertex2).sqrMagnitude;
        Vector3 pointOnEdge = Vector3.Project(point - vertex1, vertex2 - vertex1) + vertex1;
        return n / (vertex1 - vertex2).sqrMagnitude;
    }
    
    

    private void OnDrawGizmos()
    {
        for (int i = 0; i < verts.Length - 1; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(verts[i]), 0.1f);
            Vector3 currentVertex = transform.TransformPoint(verts[i]);
            Vector3 nextVertex = transform.TransformPoint(verts[i + 1]);
            Gizmos.DrawLine(currentVertex, nextVertex);
        }
    }
}
