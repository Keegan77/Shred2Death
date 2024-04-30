using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LowerConnectorFirewallCutscene : GameplayCutsceneBase
{
    List<IEnumerator> cameraTasks = new List<IEnumerator>();
    [SerializeField] private Transform keyframePoints;
    [SerializeField] private GameObject[] firewallMats;
    [SerializeField] private float cutscenePanTime;
    [SerializeField] AnimationCurve easeInOut;
    private void Start()
    {
        foreach (var wall in firewallMats)
        {
            wall.GetComponent<Renderer>().material.SetFloat("_Erosion", .9f);
        }
        cameraTasks.Add(MoveCameraToTransform(null, keyframePoints, cutscenePanTime, motionCurve:easeInOut));
        cameraTasks.Add(LowerFirewall(2));
        cameraTasks.Add(MoveCameraToTransform(null, GetOriginalParent(), cutscenePanTime, motionCurve:easeInOut));
    }
    
    public void StartCutscene()
    {
        StartCoroutine(ExecuteCameraTasks(cameraTasks, true, false));
    }
    
    private IEnumerator LowerFirewall(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            foreach (var wall in firewallMats)
            {
                wall.GetComponent<Renderer>().material.SetFloat("_Erosion",  Mathf.Lerp(.9f, 1, t / duration));
            }
            yield return null;
        }

        foreach (var wall in firewallMats)
        {
            wall.GetComponent<Collider>().enabled = false;
        }
        
    }
    
}
