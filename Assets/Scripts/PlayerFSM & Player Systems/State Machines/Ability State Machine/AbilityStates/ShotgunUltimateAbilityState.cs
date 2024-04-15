using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunUltimateAbilityState : AbilityState
{
    private float selectionRadius = 10f;
    private int numberOfBullets = 50;
    private Vector3[] pointsInCircle = new Vector3[50];
    private GameObject selectionObj;
    public ShotgunUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
        selectionObj = player.shotgunUltSelectionObj;
    }
    
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Shotgun Ultimate Entered");
        //get 50 random points in a circle around the player
        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = Random.value * Mathf.PI * 2; // random angle
            float radius = Random.value * selectionRadius; // random radius
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            pointsInCircle[i] = new Vector3(x, 0, z);
        }

        foreach (var point in pointsInCircle)
        {
            GameObject.Instantiate(selectionObj, player.transform.position + point, Quaternion.identity);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
