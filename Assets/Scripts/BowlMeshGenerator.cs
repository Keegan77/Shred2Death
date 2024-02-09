using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dreamteck.Splines;
using UnityEditor;
using UnityEngine.Serialization;

public class BowlMeshGenerator : MonoBehaviour
{
    private Collider collider;
    [SerializeField]
    [Range(0.05f, 1.0f)] float splineDetectionThreshold;
    private MeshFilter originalMeshFilter;
    private MeshFilter extrudedMeshFilter;
    private Mesh extrudedMesh;

    [SerializeField] private float extrusionHeight;

    [SerializeField] private bool closedLoop;
    
    private Vector3[] verts;
    private Vector3[] topVerts = null;
    private Vector2 heightConsiderationThreshold; 
    [SerializeField] SplineComputer vertSpline;
    
    private void Start()
    {
        originalMeshFilter = GetComponent<MeshFilter>();
        
        verts = originalMeshFilter.sharedMesh.vertices;
        verts = ConvertVertsToWorldSpace(verts);
        
        collider = GetComponent<MeshCollider>();

        topVerts = GetAllTopVertices(verts, 1);
        
        GenerateExtrudedMesh();
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

    private Vector3[] GetAllTopVertices(Vector3[] vertList, float proximityThreshold)
    {
        Dictionary<Vector3, Double> vertSampleMap = new Dictionary<Vector3, Double>();
        List<Vector3> topVertices = new List<Vector3>();

        for (int i = 0; i < vertList.Length; i++)
        {
            // Project the vertex onto the spline
            SplineSample result = vertSpline.Project(vertList[i]);

            // Calculate the distance between the vertex and the closest point on the spline
            float distance = Vector3.Distance(vertList[i], result.position);

            // If the distance is within the proximity threshold, add the vertex to the list
            if (distance <= proximityThreshold)
            {
                if (!topVertices.Contains(vertList[i]))
                {
                    topVertices.Add(vertList[i]);
                    vertSampleMap.Add(vertList[i], result.percent);
                }
            }
        }

        double threshold = .05f;
        List<Vector3> keys = new List<Vector3>(vertSampleMap.Keys);
        foreach (var key in keys)
        {
            if (vertSampleMap[key] >= 1 - threshold)
            {
                vertSampleMap[key] = 0;
            } else if (vertSampleMap[key] <= 0 + threshold)
            {
                vertSampleMap[key] = 1;
            }
        }
        topVertices = topVertices.OrderBy(v => vertSampleMap[v]).ToList();
        return topVertices.ToArray();
    }

    private void GenerateExtrudedMesh()
    {
        GameObject extrudedMeshObj = new GameObject();
        extrudedMeshObj.layer = LayerMask.NameToLayer("BowlMesh");
        extrudedMeshObj.name = "Extruded Mesh";
        MeshContainerSingleton.Instance.extrusionMeshObjects.Add(extrudedMeshObj);

        //MeshRenderer meshRenderer = extrudedMeshObj.AddComponent<MeshRenderer>();
        //meshRenderer.material = new Material(Shader.Find("Standard"));
        
        extrudedMesh = new Mesh();
        extrudedMesh.name = "Generated Mesh";
        
        extrudedMesh.vertices = GenerateExtrudedVertices(topVerts);

        extrudedMesh.triangles = GenerateExtrudedTris();
        
        extrudedMesh.RecalculateNormals();

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
            extrudedVerts[i] = baseVerts[i] + new Vector3(0, extrusionHeight, 0);
        }

        foreach (var vert in baseVerts)
        {
            allVertices.Add(vert);
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
    
     private int[] GenerateExtrudedTris()
    {
        List<int> triangles = new List<int>();
        int baseCount = extrudedMesh.vertices.Length / 2; // Assuming the first half are base vertices and the second half are extruded vertices

        // Generate triangles for the sides of the mesh
        for (int i = 0; i < baseCount; i++)
        {
            int indexUpOne = (i + 1) % baseCount;
            int extrudedIndex = i + baseCount;

            if (!closedLoop)
            {
                if (i == baseCount - 1)
                {
                    continue;
                }
            }
            

            GenerateTriangle(i, 
                             extrudedIndex,
                             indexUpOne,
                             triangles);
            
            GenerateTriangle(extrudedIndex,
                             indexUpOne + baseCount,
                             indexUpOne,
                             triangles);
            
            GenerateTriangle(indexUpOne, 
                extrudedIndex,
                i,
                triangles);
            
            GenerateTriangle(indexUpOne,
                indexUpOne + baseCount,
                extrudedIndex,
                triangles);


        }

        return triangles.ToArray();
    }

    private void GenerateTriangle(int vertIndex1, int vertIndex2, int vertIndex3, List<int> listToAddTo)
    {
        listToAddTo.Add(vertIndex1);
        listToAddTo.Add(vertIndex2);
        listToAddTo.Add(vertIndex3);
    }
    
    private void OnDrawGizmos()
    {
        if (extrudedMesh != null)
        {
            for (int i = 0; i < extrudedMesh.vertices.Length; i++)
            {
                Gizmos.DrawSphere(extrudedMesh.vertices[i], 1f);
            }
        }
    }
}
