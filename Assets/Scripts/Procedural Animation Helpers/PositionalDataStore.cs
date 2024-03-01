using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PositionalDataStore", menuName = "Procedural Animation Wizardry")]
public class PositionalDataStore : ScriptableObject 
{
    // the purpose of this asset is to store the positional data of player objects after play mode has exited.
    // this makes setting up bone positions much easier and less time consuming.
    // create one scriptable object per gun, and put the bones that you are moving in the list below

    [Tooltip("do not set manually, will be set by code based on localBones, and will be used to store the bone positions. Indices match up with localBones.")]
    [HideInInspector] public List<Vector3> targetLocalPositions;
    [HideInInspector] public List<Vector3> targetLocalEulerRotations;

    [HideInInspector] public List<Vector3> gunLocalPositions;
    [HideInInspector] public List<Vector3> gunLocalEulerRotations;
    [HideInInspector] public List<Vector3> gunSwivelPositions;
    [HideInInspector] public List<Vector3> gunSwivelEulerRotations;

}
