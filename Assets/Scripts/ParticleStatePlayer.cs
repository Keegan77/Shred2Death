using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStatePlayer : MonoBehaviour // will be extended to be flexible and potentially boilerplate
{
    ParticleSystem particleSystem;
    
    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void PlayParticle()
    {
        particleSystem.Play();
    }

    public void StopParticle()
    {
        particleSystem.Stop();
    }
    
}
