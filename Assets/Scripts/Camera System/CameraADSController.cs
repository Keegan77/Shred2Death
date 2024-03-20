using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraADSController : MonoBehaviour
{
    [SerializeField] private Transform originPoint;
    [SerializeField] private Transform ADSPoint;
    [SerializeField] AnimationCurve ADSAnimationCurve;
    [SerializeField] PlayerHealth playerHealth;
    public bool ongoingADSRoutine { get; private set; }
    public bool ADSEnabled { get; private set; }
    private bool queueStopADS;

    [SerializeField]
    private PlayerBase player;


    private void Awake()
    {
        InputRouting.Instance.input.Player.AimDownSights.started += ctx =>
        {
            if (playerHealth.IsDead()) return;
            StartCoroutine(ADS(originPoint, ADSPoint));
            //transform.position = originPoint.position;
            
            //BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(.2f, .2f));
        };
        InputRouting.Instance.input.Player.AimDownSights.canceled += ctx =>
        {
            if (playerHealth.IsDead()) return;
            queueStopADS = true;
        };
    }

    private void Update()
    {

    }


    private IEnumerator ADS(Transform from, Transform to)
    {
        //transform.position = from.position;
        //Debug.Break();
        if (ongoingADSRoutine) yield break;
        if (player.stateMachine.currentState != player.skatingState &&
            player.stateMachine.currentState != player.driftState)
        {
            BulletTimeManager.Instance.ChangeBulletTime(.2f);
        }
        ongoingADSRoutine = true;
        queueStopADS = false;
        float t = 0;
        
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * player.playerData.adsSpeed;
            transform.position = Vector3.Lerp(from.position, to.position, ADSAnimationCurve.Evaluate(t));
            yield return null;
        }
        transform.position = to.position;
        ADSEnabled = true;
        yield return new WaitUntil(() => queueStopADS);
       //BulletTimeManager.Instance.StartCoroutine(BulletTimeManager.Instance.ChangeBulletTime(1f, .2f));
       BulletTimeManager.Instance.ChangeBulletTime(1f);
        t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * player.playerData.adsSpeed;
            transform.position = Vector3.Lerp(to.position, from.position, ADSAnimationCurve.Evaluate(t));
            yield return null;
        }
        transform.position = from.position;
        queueStopADS = false;
        ongoingADSRoutine = false;
        ADSEnabled = false;
    }
}
