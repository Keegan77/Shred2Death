using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class GameplayCutsceneBase : MonoBehaviour
{
     [SerializeField] private Transform originalParent;
    public Transform GetOriginalParent()
    {
        return originalParent;
    }
    
    /// <summary>
    /// Inheritors can use this method to execute a list of camera tasks in sequence. These tasks can be any coroutines.
    /// This method also moves camera control from the player to the world, returning that control after all tasks
    /// are complete.
    /// </summary>
    /// <param name="cameraTasks"></param>
    /// <param name="disableInput"></param>
    /// <param name="freezeTime"></param>
    /// <returns></returns>
    public IEnumerator ExecuteCameraTasks(List<IEnumerator> cameraTasks, bool disableInput = true, bool freezeTime = true, float cameraFov = 40)
    {
        ActionEvents.StartedGameplayCutscene?.Invoke();
        ActionEvents.TurnOffPlayerUI?.Invoke();
        if (disableInput) InputRouting.Instance.DisableInput(); //stops player input during cutscene
        if (freezeTime) BulletTimeManager.Instance.ChangeBulletTime(0f); // freezes player for cutscene
        Helpers.MainCamera.transform.parent = null;
        if (cameraFov != 40)
        {
            Debug.Log("CHANGING FOV");
            Helpers.MainCamera.fieldOfView = cameraFov;
        }
        foreach (var task in cameraTasks)
        {
            yield return task;
        }
        if (disableInput) InputRouting.Instance.EnableInput();
        if (freezeTime) BulletTimeManager.Instance.ChangeBulletTime(1f);
        Debug.Log("FINISHED MOVIE");
        Helpers.MainCamera.transform.parent = originalParent;
        Helpers.MainCamera.transform.localPosition = Vector3.zero;
        Helpers.MainCamera.transform.localRotation = Quaternion.identity;
        ActionEvents.EndedGameplayCutscene?.Invoke();
        ActionEvents.TurnOnPlayerUI?.Invoke();
    }

    /// <summary>
    /// Can be used in gameplay cutscenes to move the camera from one position to another over time. Simply add this to
    /// the list of tasks to be executed in ExecuteCameraTasks, and fill in params. An optional animation curve can be
    /// added to smooth movement.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="startRot"></param>
    /// <param name="endTransform"></param>
    /// <param name="panTime"></param>
    /// <param name="motionCurve"></param>
    /// <returns>Coroutine</returns>
    public IEnumerator MoveCameraToTransform(Transform startTransform, Transform endTransform, float panTime,
                                     AnimationCurve motionCurve = null, bool instantCut = false, bool useCurrentPos = false, float fov = default)
    {
        Vector3 startPos = Helpers.MainCamera.transform.position;
        Quaternion startRot = Helpers.MainCamera.transform.rotation;
        if (fov != default) Helpers.MainCamera.fieldOfView = fov;
        float t = 0;
        while (t < 1)
        {
            if (instantCut)
            {
                Helpers.MainCamera.transform.position = endTransform.position;
                Helpers.MainCamera.transform.rotation = endTransform.rotation;
                yield break;
            }
            t += Time.unscaledDeltaTime / panTime;
            Helpers.MainCamera.transform.position = Vector3.LerpUnclamped(useCurrentPos ? startPos : startTransform.position,
                                                       endTransform.position,
                                                       motionCurve?.Evaluate(t) ?? t);
            Helpers.MainCamera.transform.rotation = Quaternion.LerpUnclamped(useCurrentPos ? startRot : startTransform.rotation, 
                                                          endTransform.rotation, 
                                                          motionCurve?.Evaluate(t) ?? t);
            yield return null;
        }
    }
    
    public IEnumerator MoveCameraOnSpline(SplineFollower sFollower, Transform lookAt = null, bool forwardOnSpline = true, float fov = default)
    {
        Helpers.MainCamera.transform.parent = sFollower.gameObject.transform;
        Helpers.MainCamera.transform.localPosition = Vector3.zero;
        Helpers.MainCamera.transform.localRotation = Quaternion.identity;
        
        
        if (fov != default) Helpers.MainCamera.fieldOfView = fov;
        sFollower.enabled = true;
        sFollower.direction = forwardOnSpline ? Spline.Direction.Forward : Spline.Direction.Backward;
        if (!forwardOnSpline) sFollower.SetPercent(100);
        sFollower.useUnscaledTime = true;
        sFollower.Rebuild();
        
        while (forwardOnSpline ? sFollower.result.percent < .99f : sFollower.result.percent > 0.01f)
        {
            if (lookAt != null)
            {
                Debug.Log("LOOKING AT");
                Helpers.MainCamera.transform.LookAt(lookAt);
            }
            yield return null;
        }
        Helpers.MainCamera.transform.parent = null;
    }
}
