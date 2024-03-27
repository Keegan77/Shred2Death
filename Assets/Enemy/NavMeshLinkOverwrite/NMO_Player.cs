using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMO_Player : MonoBehaviour
{
    [SerializeField]
    private NMO_AgentMover _movement;
    [SerializeField]
    private NMO_AgentAnimation _agentAnimation;


    private void Start()
    {
        _movement.OnSpeedChanged += _agentAnimation.SetSpeed;
        _movement.OnStartJump.AddListener(_agentAnimation.Jump);
        _agentAnimation.SetSpeed(0);
    }

}
