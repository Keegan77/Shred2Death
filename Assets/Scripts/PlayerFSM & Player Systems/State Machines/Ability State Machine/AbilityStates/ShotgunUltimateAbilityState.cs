using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunUltimateAbilityState : AbilityState
{
    private float selectionRadius = 7f;
    private float turnSpeed = 500;
    float abilityLifetime = 5;
    float currentLifetime = 0;
    private int amtOfSelectionObjs = 5;
    private List<GameObject> selectionObjs = new List<GameObject>();
    private List<Transform> circleTransforms = new List<Transform>();
    private float endSelectionGroundY;
    private float diffBetweenStartAndEndY;
    private Vector3 initPos;

    private float sinMagnitude = 3f;
    private float sinDistance;
    public ShotgunUltimateAbilityState(PlayerBase player, AbilityStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        currentLifetime = 0;
        selectionObjs.Clear();
        for (int i = 0; i < amtOfSelectionObjs; i++)
        {
            selectionObjs.Add(GameObject.Instantiate(player.shotgunUltSelectionObj, new Vector3(0, 25, 0),
                Quaternion.identity));
        }
        
        if (Physics.Raycast(selectionObjs[0].transform.position, Vector3.down, out RaycastHit hit, 200))
        {
            endSelectionGroundY = hit.point.y;
        }

        for (int i = 0; i < amtOfSelectionObjs; i++)
        {
            selectionObjs[i].transform.position = new Vector3(selectionObjs[i].transform.position.x,
                (selectionObjs[i].transform.position.y + endSelectionGroundY) * .5f,
                selectionObjs[i].transform.position.z);
            
            circleTransforms.Add(selectionObjs[i].transform.GetChild(0));
            circleTransforms[i].localPosition = new Vector3(selectionRadius, 0, 0);
        }
        
        diffBetweenStartAndEndY = selectionObjs[0].transform.position.y - endSelectionGroundY;
        initPos = selectionObjs[0].transform.position;
        Debug.Log("Shotgun Ultimate Entered");
        
        for (int i = 0; i < 2; i++)
        {
            player.StartCoroutine(RainBullets());
        }
    }
    
    private Vector3 GetRandomPointInCircle()
    {
        float angle = Random.value * Mathf.PI * 2; // random angle
        float radius = Random.value * selectionRadius; // random radius
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        return new Vector3(x, 50, z);
    }
    
    private IEnumerator RainBullets()
    {
        while (currentLifetime < abilityLifetime)
        {
            //var point = pointsInCircle[Random.Range(0, pointsInCircle.Length - 1)];
            var point = GetRandomPointInCircle();
            yield return new WaitForSeconds(.003f);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 200))
            {
                player.gunfireHandler.ExecuteGunshot(overrideHit:hit,
                                                     overrideStartPoint:point,
                                                     useRecoil:false,
                                                     useSound:false);
            }
        }
        
        stateMachine.SwitchState(player.intermediaryAbilityState);
    }

    public override void Exit()
    {
        base.Exit();
        foreach (var obj in selectionObjs)
        {
            GameObject.Destroy(obj);
        }
        selectionObjs.Clear();
        circleTransforms.Clear();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        currentLifetime += Time.deltaTime;
        for (int i = 0; i < selectionObjs.Count; i++)
        {
            HandleSelectionTrail(selectionObjs[i], (i + 1) % 2 == 0, i * 3f);
        }
        
    }

    private void HandleSelectionTrail(GameObject selectionObject, bool goBackward, float offset)
    {
        selectionObject.transform.Rotate(new Vector3(0, (goBackward ? -turnSpeed : turnSpeed) * Time.deltaTime, 0));
        selectionObject.transform.position = new Vector3(initPos.x, (Mathf.Sin(Time.time * sinMagnitude - offset) * diffBetweenStartAndEndY) + initPos.y, initPos.z);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
