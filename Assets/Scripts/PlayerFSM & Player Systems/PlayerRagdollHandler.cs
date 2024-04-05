using System.Collections.Generic;
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
        InputRouting.Instance.input.Debug.InitiateDeath.performed += ctx => player.stateMachine.SwitchState(player.deathState);
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
        skateboardDisable.GetComponent<SkinnedMeshRenderer>().enabled = false;
        
        Vector3 cachedPos = skateboardDisable.transform.position;
        Quaternion cachedRot = skateboardDisable.transform.rotation;
        Vector3 cachedVel = player.rb.velocity;
        
        GameObject skateboardDoll = Instantiate(newSkateboard, cachedPos + new Vector3(0, .25f, 0), cachedRot);
        Rigidbody boardDollRb = skateboardDoll.GetComponent<Rigidbody>();
        
        boardDollRb.velocity = cachedVel;
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
        Destroy(FindObjectOfType<GunfireHandler>());
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
