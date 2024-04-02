using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;

public class DissolvingController : MonoBehaviour
{
    Animator animator;
    SkinnedMeshRenderer skinnedMeshRenderer;
    VisualEffect VFXGraph;

    public UnityEvent OnDissolved;

    [Tooltip ("How long before the enemy starts dissolving?")]
    public float dieDelay;
    [Tooltip("How long does it take for the enemy to dissolve?")]
    public float disolveTime = 2f;

    private Material[] dissolveMaterials;
    void Awake()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = transform.Find("MESH_Demon").GetComponent<SkinnedMeshRenderer>();
        VFXGraph = transform.Find("ParticlesToAnimatedCharacter").GetComponent<VisualEffect>();

        if(VFXGraph != null)
        {
            VFXGraph.Stop();
            VFXGraph.gameObject.SetActive(false);
        }

        if (skinnedMeshRenderer != null)
        {
            dissolveMaterials = skinnedMeshRenderer.materials;
        }
           
     }

    // Update is called once per frame
    void Update()
    {
    }

    public void DEBUG_TestDeath ()
    {
        StartCoroutine (Dissolve ());
    }

    public IEnumerator Dissolve()
    {
        //Check to see if the required components are present.
        //If they are not, skip the dissolve step
        if (VFXGraph != null && animator != null)
        {
            //animator.SetBool ("fuckingdies", true);

            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }
        else 
        { 
            Debug.LogError ("Dissolve Components Missing"); 
            OnDissolved.Invoke ();  
            StopCoroutine (Dissolve()); 
        }

        yield return new WaitForSeconds (dieDelay);

        if (dissolveMaterials.Length > 0)
        {
            float counter = 0;

            while(dissolveMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                for(int i=0; i<dissolveMaterials.Length; i++)
                {
                    dissolveMaterials[i].SetFloat("_DissolveAmount", counter);
                }


                counter = Mathf.MoveTowards(counter, 1, 1 / disolveTime * Time.deltaTime);
                yield return null;
            }
        }

        OnDissolved.Invoke();
    }
}
