using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        SceneManager.sceneLoaded += ClearList;
    }

    private void ClearList(Scene scene, LoadSceneMode mode)
    {
        extrusionMeshObjects.Clear();
    }
}
