using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCutsceneBase : MonoBehaviour
{
    [HideInInspector] public Transform originalParent;
    private void Start()
    {
        originalParent = transform.parent;
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
    public IEnumerator ExecuteCameraTasks(List<IEnumerator> cameraTasks, bool disableInput = true, bool freezeTime = true)
    {
        if (disableInput) InputRouting.Instance.DisableInput(); //stops player input during cutscene
        if (freezeTime) BulletTimeManager.Instance.ChangeBulletTime(0f); // freezes player for cutscene
        transform.parent = null;
        foreach (var task in cameraTasks)
        {
            yield return task;
        }
        if (disableInput) InputRouting.Instance.EnableInput();
        if (freezeTime) BulletTimeManager.Instance.ChangeBulletTime(1f);
        transform.parent = originalParent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
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
    public IEnumerator LerpTransform(Vector3 startPos, Quaternion startRot, Transform endTransform, float panTime,
                                     AnimationCurve motionCurve = null)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / panTime;
            transform.position = Vector3.LerpUnclamped(startPos,
                                                       endTransform.position,
                                                       motionCurve?.Evaluate(t) ?? t);
            transform.rotation = Quaternion.LerpUnclamped(startRot, 
                                                          endTransform.rotation, 
                                                          motionCurve?.Evaluate(t) ?? t);
            yield return null;
        }
    }
}
