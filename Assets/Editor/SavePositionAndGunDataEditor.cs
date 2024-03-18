#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SavePositionAndGunData))]
public class SavePositionAndGunDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SavePositionAndGunData myScript = (SavePositionAndGunData)target;
        if (GUILayout.Button("Set Positions"))
        {
            myScript.SetPositions();
        }
        if (GUILayout.Button("Preview Positions"))
        {
            myScript.RebuildRig();
        }
    }
}
#endif
