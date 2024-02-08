using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoundTesting : MonoBehaviour
{
    private Collider collider;
    private MeshFilter originalMeshFilter;
    private MeshFilter extrudedMeshFilter;
    private Mesh extrudedMesh;

    public Vector3[] vertexGroups;
    public List<Vector3>[] vertList;
    public Vector3[] verts;
    public Vector3[] topVerts;
    public Vector3[] normals;
    private float topYVerticeHeight;
    Vector2 heightConsiderationThreshold; // should be equal to the top vertice height + and minus a certain amount, which we can define
    
    private void Start()
    {
        originalMeshFilter = GetComponent<MeshFilter>();
        verts = originalMeshFilter.sharedMesh.vertices;
        verts = ConvertVertsToWorldSpace(verts);
        
        collider = GetComponent<MeshCollider>();
        topYVerticeHeight = GetTopVerticeHeight(verts);
        topVerts = GetAllTopVertices(verts, new Vector2(topYVerticeHeight - 0.2f, topYVerticeHeight + 0.2f));
        Debug.Log($"top vertice height: {topYVerticeHeight} ");
        //GenerateExtrudedMesh();
        //mesh.triangles = collider.GetComponent<MeshCollider>().sharedMesh.triangles;
    }

    private void Update()
    {
        //Debug.Log(collider.bounds);
    }
    
    private Vector3[] ConvertVertsToWorldSpace(Vector3[] verts)
    {
        Vector3[] worldSpaceVerts = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            worldSpaceVerts[i] = transform.TransformPoint(verts[i]);
        }
        return worldSpaceVerts;
    }

    private float GetTopVerticeHeight(Vector3[] vertList)
    {
        float topVerticeHeight = -Mathf.Infinity;
        foreach (var vert in vertList)
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
        GameObject extrudedMeshObj = new GameObject();
        extrudedMeshObj.name = "Extruded Mesh";

        MeshRenderer meshRenderer = extrudedMeshObj.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        
        extrudedMesh = new Mesh();
        extrudedMesh.name = "fucking awesome cool extruded mesh";
        
        extrudedMesh.vertices = GenerateExtrudedVertices(topVerts);
        
        extrudedMesh.RecalculateNormals();
        normals = extrudedMesh.normals;
        extrudedMesh.RecalculateBounds();
        
        extrudedMeshFilter = extrudedMeshObj.AddComponent<MeshFilter>();
        extrudedMeshFilter.mesh = extrudedMesh;
        
        extrudedMeshObj.AddComponent<MeshCollider>();
    }

    private Vector3[] GenerateExtrudedVertices(Vector3[] baseVerts)
    {
        List<Vector3> allVertices = new List<Vector3>();
        Vector3[] extrudedVerts = new Vector3[baseVerts.Length];
        
        //baseVerts = SortVerticesClockwise(baseVerts);
        
        for (int i = 0; i < extrudedVerts.Length; i++)
        {
            extrudedVerts[i] = baseVerts[i] + new Vector3(0, 30, 0);
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
        } // we use two foreach loops here so points get added to the array in the correct order for triangle generation
        return allVertices.ToArray();
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
        if (topVerts != null)
        {
            for (int i = 0; i < topVerts.Length; i++)
            {
                Gizmos.DrawSphere(topVerts[i], 1f);
                //Gizmos.DrawLine(extrudedMesh.vertices[i], extrudedMesh.vertices[i] + normals[i]);
            }
        }
    }
}
