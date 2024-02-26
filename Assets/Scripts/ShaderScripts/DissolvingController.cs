using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class DissolvingController : MonoBehaviour
{
    public Animator animator;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.02f;
    public float refreshRate = 0.05f;
    public float dieDelay = 0.2f;
    
    public bool deadlydeath;

    private Material[] dissolveMaterials;
    void Start()
    {
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
        if (deadlydeath == true)
        {
            StartCoroutine(Dissolve());
        }
    }

    IEnumerator Dissolve()
    {
        if (animator != null)
        {
            animator.SetBool("fuckingdies", true);
        }
        yield return new WaitForSeconds(dieDelay);

        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }
        
        float counter = 0;

        if (dissolveMaterials.Length > 0)
        {
            while(dissolveMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;

                for(int i=0; i<dissolveMaterials.Length; i++)
                {
                    dissolveMaterials[i].SetFloat("_DissolveAmount", counter);
                }
         
                yield return new WaitForSeconds (refreshRate);
            }
        }

        Destroy(gameObject);
    }
}
