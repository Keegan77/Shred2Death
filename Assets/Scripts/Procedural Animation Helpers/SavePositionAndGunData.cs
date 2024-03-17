#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;


public class SavePositionAndGunData : MonoBehaviour
{
    public Transform[] targets;
    public Transform[] guns;
    public Transform[] gunSwivels;
    public PositionalDataStore dataStore;
    [SerializeField] RigBuilder rigBuilder;
    
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += HandlePlayModeState;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeState;

    }
    private void HandlePlayModeState(PlayModeStateChange state)
    {
        dataStore.targetLocalPositions.Clear();
        dataStore.targetLocalEulerRotations.Clear();
        dataStore.gunLocalPositions.Clear();
        dataStore.gunLocalEulerRotations.Clear();
        dataStore.gunSwivelPositions.Clear();
        
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var target in targets)
            {
                dataStore.targetLocalPositions.Add(target.localPosition);
                dataStore.targetLocalEulerRotations.Add(target.localEulerAngles);
            }

            foreach (var gun in guns)
            {
                dataStore.gunLocalPositions.Add(gun.localPosition);
                dataStore.gunLocalEulerRotations.Add(gun.localEulerAngles);
            }
            
            foreach (var swivel in gunSwivels)
            {
                dataStore.gunSwivelPositions.Add(swivel.localPosition);
                dataStore.gunSwivelEulerRotations.Add(swivel.localEulerAngles);
            }
            // Your code here
        }
        
    }
    public void SetPositions()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].localPosition = dataStore.targetLocalPositions[i];
            targets[i].localEulerAngles = dataStore.targetLocalEulerRotations[i];
        }

        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].localPosition = dataStore.gunLocalPositions[i];
            guns[i].localEulerAngles = dataStore.gunLocalEulerRotations[i];
        }

        for (int i = 0; i < gunSwivels.Length; i++)
        {
            gunSwivels[i].localPosition = dataStore.gunSwivelPositions[i];
            gunSwivels[i].localEulerAngles = dataStore.gunSwivelEulerRotations[i];
        }  
    }

    public void RebuildRig()
    {
        rigBuilder.Build();
    }

    
}
#endif
