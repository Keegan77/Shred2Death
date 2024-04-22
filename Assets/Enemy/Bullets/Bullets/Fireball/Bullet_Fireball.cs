using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Bullet_Fireball : Enemy_Bullet
{
    public override void StartBullet ()
    {
        base.StartBullet ();
        GetComponent<Rigidbody> ().velocity = transform.forward * speed;
    }
}
