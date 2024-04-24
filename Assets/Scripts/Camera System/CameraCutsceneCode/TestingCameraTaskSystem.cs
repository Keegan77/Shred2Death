using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

public class TestingCameraTaskSystem : GameplayCutsceneBase
{
    public Transform newKeyframe;
    public AnimationCurve cameraSpringCurve;
    [SerializeField] private float panTime;
    [SerializeField] private float stayAtIvalTime;
    [SerializeField] SplineFollower splineFollower;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        List<IEnumerator> cameraTasks = new List<IEnumerator>();
        Vector3 startPos = Helpers.MainCamera.transform.position;
        Quaternion startRot = Helpers.MainCamera.transform.rotation;

        cameraTasks.Add(LerpTransform(startPos, startRot, splineFollower.transform, panTime, cameraSpringCurve));
        cameraTasks.Add(MoveCameraOnSpline(splineFollower, forwardOnSpline:false));
        //cameraTasks.Add(new WaitForSecondsRealtime(stayAtIvalTime));
        cameraTasks.Add(LerpTransform(splineFollower.transform.position, splineFollower.transform.rotation,
                              GetOriginalParent(), panTime, cameraSpringCurve));

        yield return ExecuteCameraTasks(cameraTasks);
    }
}
