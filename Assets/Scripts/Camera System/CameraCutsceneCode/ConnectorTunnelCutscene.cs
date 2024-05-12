using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorTunnelCutscene : GameplayCutsceneBase
{
    List<IEnumerator> cameraTasks = new List<IEnumerator>();
    [SerializeField]
    float stallTime = 5;
    [SerializeField] Transform[] goToTransforms;
    [SerializeField] AnimationCurve easeCurve;
    private void Start()
    {
        cameraTasks.Add(MoveCameraToTransform(null, goToTransforms[0], 0, instantCut:true, fov:35));
        cameraTasks.Add(new WaitForSecondsRealtime(stallTime));
        StartCoroutine(LookAtPlayer());
        cameraTasks.Add(MoveCameraToTransform(null, GetOriginalParent(), 1.5f, motionCurve: easeCurve));

        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false, cameraFov: 90));
    }
    
    private IEnumerator LookAtPlayer()
    {
        float t = 0;
        PlayerBase player = FindObjectOfType<PlayerBase>();
        while (t < stallTime)
        {
            t += Time.deltaTime;
            Helpers.MainCamera.transform.LookAt(player.transform);
            yield return null;
        }
    }
}
