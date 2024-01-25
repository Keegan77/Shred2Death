using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [Serializable] class Row
    {
        [SerializeField] GameObject enemy;
        [SerializeField] int count = 1;
    }

    [SerializeField]
    Row[] enemies;
    

    //EnemyCount()
}
