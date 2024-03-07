using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Bullet_Fireball : Enemy_Bullet
{
    [Header("Fireball")]
    public float changeThisLater = 0;

    private void FixedUpdate ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
