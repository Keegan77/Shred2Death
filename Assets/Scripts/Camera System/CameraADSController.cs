using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraADSController : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    [SerializeField] private Transform ADSPoint;
    [SerializeField] AnimationCurve ADSAnimationCurve;
    private bool ADSEnabled;
    private bool queueStopADS;

    [SerializeField]
    private PlayerBase player;


    private void Awake()
    {
        InputRouting.Instance.input.Player.AimDownSights.started += ctx =>
        {
            StartCoroutine(ADS(originPoint, ADSPoint));
            if (player.stateMachine.currentState.GetType() == typeof(PlayerSkatingState)) return;
            BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(.2f, .2f));
        };
        InputRouting.Instance.input.Player.AimDownSights.canceled += ctx =>
        {
            queueStopADS = true;
        };
    }
    

    private IEnumerator ADS(Transform from, Transform to)
    {
        if (ADSEnabled) yield break;
        ADSEnabled = true;
        queueStopADS = false;
        float t = 0;
        
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * player.playerData.adsSpeed;
            transform.position = Vector3.Lerp(from.position, to.position, ADSAnimationCurve.Evaluate(t));
            yield return null;
        }
        
        yield return new WaitUntil(() => queueStopADS);
        BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(1f, .2f));
        t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * player.playerData.adsSpeed;
            transform.position = Vector3.Lerp(to.position, from.position, ADSAnimationCurve.Evaluate(t));
            yield return null;
        }
        queueStopADS = false;
        ADSEnabled = false;
    }
}
