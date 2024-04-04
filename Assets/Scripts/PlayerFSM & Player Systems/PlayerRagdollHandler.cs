using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRagdollHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject hips;
    [SerializeField] private GameObject playerRig;
    [Tooltip("Put the player's singular capsule collider here")]
    [SerializeField] private Collider colliderToDisable;

    [SerializeField] private Animator animatorToDisable;

    [SerializeField] private GameObject skateboardDisable;
    [SerializeField] private GameObject newSkateboard;

    [SerializeField] private PlayerBase player;
    
    public List<GameObject> ragdollLimbs;

    public bool ragdollEnabled;

    private void Start()
    {
        FindRagdollLimbs();
    }

    private void OnEnable()
    {
        InputRouting.Instance.input.Player.Brake.performed += ctx => player.stateMachine.SwitchState(player.deathState);
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (ragdollEnabled)
        {
            player.transform.position = hips.transform.position;
        }
    }

    public void ActivateRagdoll()
    {
        ragdollEnabled = true;
        colliderToDisable.enabled = false;
        animatorToDisable.enabled = false;
        
        
        Vector3 cachedPos = skateboardDisable.transform.position;
        Quaternion cachedRot = skateboardDisable.transform.rotation;
        skateboardDisable.GetComponent<SkinnedMeshRenderer>().enabled = false;
        GameObject newGuy = Instantiate(newSkateboard, cachedPos + new Vector3(0, .25f, 0), cachedRot);
        Rigidbody newRb = newGuy.transform.AddComponent<Rigidbody>();
        Vector3 cachedVel = player.rb.velocity;
        newRb.velocity = cachedVel;
        //skateboard.transform.position = cachedPos + new Vector3(0, 5.43f, 0);
        playerRig.transform.parent = null;
        
        //skateboardCollider.enabled = isEnabled;
        
        foreach (var limb in ragdollLimbs)
        {
            limb.GetComponent<Collider>().enabled = true;
            limb.GetComponent<Rigidbody>().isKinematic = false;
            limb.GetComponent<Rigidbody>().velocity = cachedVel;
        }
        Destroy(player.GetComponent<ConstantForce>());
        Destroy(player.rb);
    }
    
    private void FindRagdollLimbs()
    {
        var ragdollColls = hips.GetComponentsInChildren<Collider>();

        foreach (Collider coll in ragdollColls)
        {
            ragdollLimbs.Add(coll.gameObject);
        }
    }
    
    
}
