using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientCamera : MonoBehaviour
{
    [SerializeField] private Transform mainRotationTransform;
    [SerializeField] private Transform additionalRotationTransform;
    [SerializeField] private PlayerBase player;

    [SerializeField] float baseSlerpSpeed;
    private Vector3 combinedRotation;
    [SerializeField] RotateWithMouse rotateWithMouseComponent;
    
    private delegate void OrientationDelegate();

    // Update is called once per frame
    private void Awake()
    {
        //ActionEvents.OnBehaviourStateSwitch += Enter;

    }

    void Enter(Type stateType)
    {
        /*switch (stateType.Name)
        {
            case nameof(PlayerHalfpipeState):
                rotateWithMouseComponent.SetRotation(new Vector3(0, 0, 0));
                break;
            // Add more cases as needed
        }*/
    }
    
    void LateUpdate()
    {
        //combinedRotation = mainRotationTransform.eulerAngles + additionalRotationTransform.localEulerAngles;
        var currentStateType = player.stateMachine.currentState.GetType();

        if (currentStateType == typeof(PlayerDriftState))
        {
            OrientToForward(); //default camera orientation state
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
    
    void OrientToForward()
    {
        //var targetRotation = Quaternion.Euler(0, combinedRotation.y, 0);

        // Slerp from the current rotation to the target rotation
        SlerpToNewRotation(Quaternion.LookRotation(player.transform.forward), baseSlerpSpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
    
    void SlerpToNewRotation(Quaternion targetRotation, float slerpSpeed)
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, slerpSpeed * Time.unscaledDeltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
    
    
}

