using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NMO_AgentMover : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _Agent;

    public event Action<float> OnSpeedChanged;

    private bool _onNavMeshLink = false;

    //[SerializeField] private float _jumpDuration = 0.8f;
    [SerializeField] private float _jumpWait = 0.0f;
    [SerializeField] private float _jumpLand = 0.0f;

    public UnityEvent OnLand, OnStartJump;

    private void Start()
    {
        _Agent.autoTraverseOffMeshLink = false;
    }


    public void SetDestination(Vector3 destination)
    {
        if (_onNavMeshLink)
            return;

        _Agent.destination = destination;
    }

    private void Update()
    {
        OnSpeedChanged?.Invoke(
            Mathf.Clamp01(_Agent.velocity.magnitude / _Agent.speed));
        
        if (_Agent.isOnOffMeshLink && _onNavMeshLink == false)
        {
            StartNavMeshLinkMovement();
        }
        //if ( _onNavMeshLink && transform.position != _Agent.currentOffMeshLinkData.endPos )
        //{
        //    FaceTarget(_Agent.currentOffMeshLinkData.endPos);
        //}
    }

    private void StartNavMeshLinkMovement()
    {
        _onNavMeshLink = true;
        NavMeshLink link = (NavMeshLink)_Agent.navMeshOwner;
        NMO_Spline spline = link.GetComponentInChildren<NMO_Spline> ();
        NMO_NavMeshLinkSpline linkdata = link.GetComponent<NMO_NavMeshLinkSpline> ();

        PerformJump(link, spline, linkdata);
    }

    private void PerformJump(NavMeshLink link, NMO_Spline spline, NMO_NavMeshLinkSpline linkdata)
    {
        bool reverseDirection = CheckIfJumpingFromEndToStart(link);
        StartCoroutine(MoveOnOffMeshLink(spline, reverseDirection, linkdata));

        OnStartJump?.Invoke();
    }

    private bool CheckIfJumpingFromEndToStart(NavMeshLink link)
    {
        Vector3 startPosWorld
            = link.gameObject.transform.TransformPoint(link.startPoint);
        Vector3 endPosWorld
            = link.gameObject.transform.TransformPoint(link.endPoint);

        float distancePlayerToStart 
            = Vector3.Distance(_Agent.transform.position, startPosWorld);
        float distancePlayerToEnd 
            = Vector3.Distance(_Agent.transform.position, endPosWorld);


        return distancePlayerToStart > distancePlayerToEnd;
    }


    private IEnumerator MoveOnOffMeshLink(NMO_Spline spline, bool reverseDirection, NMO_NavMeshLinkSpline linkdata)
    {
        Enemy e =  GetComponent<Enemy> ();
        e.animator.Play ("JUMP");

        yield return new WaitForSeconds (_jumpWait);

        float currentTime = 0;
        Vector3 agentStartPosition = _Agent.transform.position;


        while (currentTime < linkdata._jumpDuration )
        {

            currentTime += Time.deltaTime;

            float amount = Mathf.Clamp01(currentTime / linkdata._jumpDuration);
            amount = reverseDirection ? 1 - amount : amount;

            _Agent.transform.position =
                reverseDirection ?
                spline.CalculatePositionCustomEnd(amount, agentStartPosition)
                : spline.CalculatePositionCustomStart(amount, agentStartPosition);

            yield return new WaitForEndOfFrame();
        }

        e.animator.Play ("JUMP_LAND");
        yield return new WaitForSeconds (_jumpLand);

        _Agent.CompleteOffMeshLink();

        e.animator.Play (e.stateMachine.stateCurrent.animationEnter);

        OnLand?.Invoke();
        yield return new WaitForSeconds(0.1f);
        _onNavMeshLink = false;

    }


    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation 
            = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation 
            = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

}
