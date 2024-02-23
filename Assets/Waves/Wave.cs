using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [Serializable] public class Row
    {
        public GameObject enemy;
        public GameObject spawnPoint;
        [Range(1, 100)]
        public int count = 1;
        [Range(1, 10)]
        public float interval = 1.0f;
        public float spawnDelay = 0.0f;
    }

    [SerializeField]
    Row[] enemies;
    
    public Row[] getEnemies ()
    {
        return enemies;
    }

    public int getEnemyCount ()
    {
        int c = 0;

        foreach (Row r in enemies)
        {
            c += r.count;
        }

        return c;
    }
}
