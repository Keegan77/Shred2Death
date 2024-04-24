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
    private void OnTriggerEnter(Collider other)
    {
        if (cutscenePlayed) return;
        cutscenePlayed = true;
        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false));
        Debug.Log("Cutscene played");
    }
}
