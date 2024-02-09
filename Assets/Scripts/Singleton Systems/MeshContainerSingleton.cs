using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshContainerSingleton : MonoBehaviour
{
    public static MeshContainerSingleton Instance;
    
    //make a dictionary of lists, where the key is the collider and the value is the list of vertices
    
    public List<GameObject> extrusionMeshObjects;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
