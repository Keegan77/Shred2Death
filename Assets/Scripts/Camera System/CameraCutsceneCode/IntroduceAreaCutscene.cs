using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

public class IntroduceAreaCutscene : GameplayCutsceneBase
{
    private bool cutscenePlayed;
    List<IEnumerator> cameraTasks;
    [SerializeField] private TextMeshProUGUI areaTextObj;
    [SerializeField] private string areaName;
    [SerializeField] private SplineFollower[] splinesToFollow;
    [Tooltip("Sometimes, you want a camera to look at an object instead of looking in the direction of the spline. " +
             "Add the index of the spline from splinesToFollow that you want to look at an object, and these indices will" +
             "use the lookAtTransforms in the order that they are placed in the array.")]
    [SerializeField] private List<int> indicesThatUseLookAt;
    [SerializeField] private Transform[] lookAtTransforms;

    private Camera camera;
    [SerializeField] private AnimationCurve easeInOut;

    private void Awake()
    {
        cameraTasks = new List<IEnumerator>();
        cutscenePlayed = false;
        camera = Helpers.MainCamera;
        int currentLookAt = -1; //incremented every time we have a spline that needs to look at the next transform in the
                                //lookAtTransforms array, as to access the next element in the array
        
        for (int i = 0; i < splinesToFollow.Length; i++)
        {
            if (i == splinesToFollow.Length - 1) //for the last spline, which is around the player, we want to do this custom task
            {
                cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i], lookAtTransforms[^1], fov:35, forwardOnSpline:false));
                continue;
            }

            if (indicesThatUseLookAt.Contains(i))
            {
                currentLookAt++;
                cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i], lookAtTransforms[currentLookAt]));
                continue;
            }

            cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i]));
        }
        cameraTasks.Add(MoveCameraToTransform(null, GetOriginalParent(), 2f, easeInOut, fov:35));
        //the last camera zoom
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (cutscenePlayed) return;
        cutscenePlayed = true;
        areaTextObj.text = areaName;
        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false, cameraFov:80, showZoneTitle:true));
        Debug.Log("Cutscene played");
    }
}
