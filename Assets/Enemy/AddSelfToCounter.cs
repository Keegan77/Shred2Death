using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSelfToCounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EnemyCounterSingleton.Instance.enemyCount++;
    }

    private void OnDestroy()
    {
        EnemyCounterSingleton.Instance.enemyCount--;
    }
}
