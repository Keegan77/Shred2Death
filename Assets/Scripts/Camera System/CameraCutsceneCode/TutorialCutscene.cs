using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialCutscene : GameplayCutsceneBase
{
    [SerializeField] private List<SplineFollower> sequentialSplines;
    [SerializeField] private AnimationCurve easeOut;
    [SerializeField] private TextMeshProUGUI areaText;
    [SerializeField] private InputAction tutorialAction;
    [SerializeField] private GameObject startingText;
    private IEnumerator Start()
    {
        yield return null;
        areaText.text = "Tunnel of Teachings";
        List<IEnumerator> cameraTasks = new List<IEnumerator>();
        Debug.Log("Starting cutscene");
        //cameraTasks.Add(MoveCameraToTransform(null,));
        cameraTasks.Add(MoveCameraOnSpline(sequentialSplines[0]));
        cameraTasks.Add(MoveCameraToTransform(null, GetOriginalParent(), 2f, easeOut, fov:35, rotationMultiplier:1));
        yield return StartCoroutine(ExecuteCameraTasks(cameraTasks, disableInput:true, freezeTime:false, showZoneTitle:true));
        ActionEvents.FreezeAndWaitForInput?.Invoke(tutorialAction, startingText);
    }
}
