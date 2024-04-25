using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Enemy_State))]
public class Editor_EnemyState : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
    }

    //private void OnSceneGUI ()
    //{

    //    DrawDefaultInspector();
    //}
}
#endif