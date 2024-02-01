using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


//This script is attached to the NavMesh object in a level.
//It takes the buildsettings of each of the navmeshSurfaces in the level
//and gives them to a static variable in the enemy class.
//This lets me access agent settings parameters because APPARENTLY, you can't
//do this shit directly through a navmesh agent
public class SetNavMeshSettings : MonoBehaviour
{
    public NavMeshBuildSettings[] arraySettings;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshSurface[] surfaces = GetComponents<NavMeshSurface>();
        arraySettings = new NavMeshBuildSettings[surfaces.Length];

        Debug.Log ($"NavmeshSurfaces: {surfaces.Length}");

        for (int i = 0; i < surfaces.Length; i++)
        {
            arraySettings[i] = surfaces[i].GetBuildSettings();
        }

        Enemy.agentSettings = arraySettings;
    }
}
