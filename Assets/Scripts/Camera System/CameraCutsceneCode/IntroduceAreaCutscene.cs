using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class IntroduceAreaCutscene : GameplayCutsceneBase
{
    private bool cutscenePlayed;
    List<IEnumerator> cameraTasks;
    [SerializeField] private SplineFollower[] splinesToFollow;

    [SerializeField] private Transform[] lookAtTransforms;

    private Camera camera;
    [SerializeField] private AnimationCurve easeInOut;

    private void Awake()
    {
        cameraTasks = new List<IEnumerator>();
        cutscenePlayed = false;
        camera = Helpers.MainCamera;

        for (int i = 0; i < splinesToFollow.Length; i++)
        {
            if (i == 1)
            {
                cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i], lookAtTransforms[0]));
                continue;
            }
            if (i == 3)
            {
                cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i], lookAtTransforms[1], fov:35, forwardOnSpline:false));
                continue;
            }
            cameraTasks.Add(MoveCameraOnSpline(splinesToFollow[i]));
            Debug.Log("Added follower to camera tasks");
        }
        cameraTasks.Add(MoveCameraToTransform(null, GetOriginalParent(), 2f, easeInOut, useCurrentPos:true, fov:35));
        
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false, cameraFov:80));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cutscenePlayed) return;
        cutscenePlayed = true;
        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false, cameraFov:80));
        Debug.Log("Cutscene played");
    }
}
