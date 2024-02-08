using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexContainer : MonoBehaviour
{
    public static VertexContainer Instance;
    
    //make a dictionary of lists, where the key is the collider and the value is the list of vertices
    
    public Dictionary<GameObject, List<Vector3>> objectVerticeMap = new Dictionary<GameObject, List<Vector3>>();
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
