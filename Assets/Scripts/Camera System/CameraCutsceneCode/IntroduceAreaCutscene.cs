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
    private Camera camera;

    private void Awake()
    {
        cameraTasks = new List<IEnumerator>();
        cutscenePlayed = false;
        camera = Helpers.MainCamera;
        foreach (SplineFollower follower in splinesToFollow)
        {
            cameraTasks.Add(MoveCameraOnSpline(follower));
            Debug.Log("Added follower to camera tasks");
        }
        
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
