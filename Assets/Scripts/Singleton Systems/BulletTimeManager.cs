using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeManager : MonoBehaviour
{
    public static BulletTimeManager Instance;
    public float bulletTimeScale;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Update()
    {
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    //todo: create list of items to not be affected by bullet time
    
    public IEnumerator ChangeBulletTime(float timeScale, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            Time.timeScale = Mathf.Lerp(Time.timeScale, timeScale, t);
            
            yield return null;
        }
    }
    
}
