using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Type_Flying : MonoBehaviour
{
    Enemy_Flying e;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake ()
    {
        e = transform.parent.GetComponent<Enemy_Flying>();
    }
}
