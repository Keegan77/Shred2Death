
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Your Class
public class enumInspector : MonoBehaviour
{
    public enum MoveType { AutoMove, Waypoints };
    public MoveType moveType;
    //Auto Move variables
    public Vector3 autoMoveDir;
    public float autoMoveSpeed;
    //waypoint varialbes
    public Vector3[] waypoints;
    public bool loopAtEnd;
} //end of class
  //Custom inspector starts here
#if UNITY_EDITOR
[CustomEditor (typeof (enumInspector))]
public class enumInspectorEditor : Editor
{

    public override void OnInspectorGUI ()
    {
        //cast target
        var enumScript = target as enumInspector;
        //Enum drop down
        enumScript.moveType = (enumInspector.MoveType) EditorGUILayout.EnumPopup (enumScript.moveType);
        //switch statement for different variables
        switch ( enumScript.moveType )
        {
            //AutoMove
            case enumInspector.MoveType.AutoMove:
                enumScript.autoMoveDir = EditorGUILayout.Vector3Field ("Direction", enumScript.autoMoveDir); //Vector3 example
                enumScript.autoMoveSpeed = EditorGUILayout.FloatField ("Speed", enumScript.autoMoveSpeed); //float example
                break;
            //waypoint
            case enumInspector.MoveType.Waypoints:
                enumScript.loopAtEnd = EditorGUILayout.Toggle ("Loop", enumScript.loopAtEnd);//bool example
                                                                                             //array example
                SerializedProperty waypointsProperty = serializedObject.FindProperty("waypoints"); //get array as Serialized Property
                EditorGUI.BeginChangeCheck (); //Check if the array inspector is dropped down
                EditorGUILayout.PropertyField (waypointsProperty, true); //array example (works with any Serialized property)
                if ( EditorGUI.EndChangeCheck () ) //End Array inspector dropped down
                    serializedObject.ApplyModifiedProperties ();
                break;
        }//end switch
    }
}//end inspectorclass
#endif
