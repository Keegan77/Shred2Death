using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CameraSpringMovement : MonoBehaviour
{
    public Transform targetTransform;
    public Transform newKeyframe;
    public AnimationCurve cameraSpringCurve;
    [SerializeField] private float panTime;
    [SerializeField] private float stayAtIvalTime;
    [SerializeField] private float rotationSpeedMult;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5);
        MoveCameraToNewTransformOverTime(newKeyframe, panTime, stayAtIvalTime);
    }

    public async Task MoveCameraToNewTransformOverTime(Transform newTransform, float panTime, float stayAtIvalTime)
    {
        Transform cachedParent = transform.parent.transform;
        transform.parent = null;
        Vector3 startPos = transform.position;
        Vector3 startRot = transform.eulerAngles;

        await LerpTransform(startPos, newTransform, Quaternion.Euler(startRot), panTime);

        await Task.Delay(TimeSpan.FromSeconds(stayAtIvalTime));

        await LerpTransform(newTransform.position, cachedParent, newTransform.rotation, panTime);

        transform.parent = cachedParent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private async Task LerpTransform(Vector3 startPos, Transform endTransform, Quaternion startRot, float panTime)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / panTime;
            transform.position = Vector3.LerpUnclamped(startPos, endTransform.position, cameraSpringCurve.Evaluate(t));
            transform.rotation = Quaternion.LerpUnclamped(startRot, endTransform.rotation, cameraSpringCurve.Evaluate(t));
            await Task.Yield();
        }
    }
}
