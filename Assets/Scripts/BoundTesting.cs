using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoundTesting : MonoBehaviour
{
    private Collider collider;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private GameObject childObject;
    protected MeshFilter extrudedMeshFilter;
    private Mesh extrudedMesh;


    public Vector3[] verts;
    public Vector3[] topVerts;
    public Vector3[] extrudedMeshVerts;
    private float topYVerticeHeight;
    Vector2 heightConsiderationThreshold; // should be equal to the top vertice height + and minus a certain amount, which we can define
    
    private void Start()
    {
        verts = meshFilter.sharedMesh.vertices;
        collider = GetComponent<MeshCollider>();
        topYVerticeHeight = GetTopVerticeHeight();
        topVerts = GetAllTopVertices(verts, new Vector2(topYVerticeHeight - 0.1f, topYVerticeHeight + 0.1f));
        Debug.Log($"top vertice height: {topYVerticeHeight} ");
        GenerateExtrudedMesh();
        //mesh.triangles = collider.GetComponent<MeshCollider>().sharedMesh.triangles;
    }

    private void Update()
    {
        //Debug.Log(collider.bounds);
    }

    private float GetTopVerticeHeight()
    {
        float topVerticeHeight = 0;
        foreach (var vert in verts)
        {
            if (vert.y > topVerticeHeight)
            {
                topVerticeHeight = vert.y;
            }
        }
        return topVerticeHeight;
    }

    private Vector3[] GetAllTopVertices(Vector3[] vertList, Vector2 heightConsiderationThreshold)
    {
        List<Vector3> topVertices = new List<Vector3>();

        for (int i = 0; i < vertList.Length; i++)
        {
            if (vertList[i].y.IsInRangeOf(heightConsiderationThreshold.x, heightConsiderationThreshold.y))
            {
                if (!topVertices.Contains(vertList[i]))
                {
                    topVertices.Add(vertList[i]);
                    Debug.Log(i);
                }
            }
        }
        return topVertices.ToArray();
    }

    private void GenerateExtrudedMesh()
    {
        extrudedMesh = new Mesh();
        extrudedMesh.name = "fucking awesome cool extruded mesh";
        extrudedMesh.vertices = GenerateExtrudedVertices();
        extrudedMeshVerts = GenerateExtrudedVertices();
        extrudedMesh.triangles = GenerateExtrudedTris();
        extrudedMesh.RecalculateNormals();
        extrudedMesh.RecalculateBounds();
        
        extrudedMeshFilter = childObject.AddComponent<MeshFilter>();
        extrudedMeshFilter.mesh = extrudedMesh;
    }

    private Vector3[] GenerateExtrudedVertices()
    {
        List<Vector3> allVertices = new List<Vector3>();
        Vector3[] baseVerts;
        baseVerts = topVerts;
        Vector3[] extrudedVerts = new Vector3[baseVerts.Length];
        
        for (int i = 0; i < extrudedVerts.Length; i++)
        {
            extrudedVerts[i] = baseVerts[i] + new Vector3(0, 1, 0);
        }

        foreach (var vert in baseVerts)
        {
            if (!allVertices.Contains(vert))
            {
                allVertices.Add(vert);
            }
        }
        foreach (var vert in extrudedVerts)
        {
            if (!allVertices.Contains(vert))
            {
                allVertices.Add(vert);
            }
        }
        return allVertices.ToArray();
    }

    private int[] GenerateExtrudedTris()
    {
        extrudedMeshVerts = GenerateExtrudedVertices();
        List<int> triangles = new List<int>();
        int baseCount = extrudedMeshVerts.Length / 2; // Assuming the first half are base vertices and the second half are extruded vertices

        // Generate triangles for the sides of the mesh
        for (int i = 0; i < baseCount; i++)
        {
            int nextIndex = (i + 2) % baseCount;
            int extrudedIndex = i + baseCount;
            int extrudedNextIndex = nextIndex + baseCount;

            // Triangle 1
            triangles.Add(i);
            triangles.Add(nextIndex);
            triangles.Add(extrudedNextIndex);

            // Triangle 2
            triangles.Add(nextIndex);
            triangles.Add(extrudedIndex);
            triangles.Add(extrudedNextIndex);
        }

        return triangles.ToArray();
    }
    
    
    
    
    float GetSqrMagFromEdge(Vector3 vertex1, Vector3 vertex2, Vector3 point)
    {
        float n = Vector3.Cross(point - vertex1, point - vertex2).sqrMagnitude;
        return n / (vertex1 - vertex2).sqrMagnitude;
    }
    
    private Vector3 ClosestPointOnEdge(Vector3 vertex1, Vector3 vertex2, Vector3 point)
    {
        return Vector3.Project(point - vertex1, vertex2 - vertex1) + vertex1;
    }
    
    

    private void OnDrawGizmos()
    {
        if (extrudedMeshVerts != null)
        {
            for (int i = 0; i < extrudedMeshVerts.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(extrudedMeshVerts[i]), 1f);
            }
        }
    }
}
