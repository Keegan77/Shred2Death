using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class dev_MovePlayerDummy : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed;
    public float driftSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate ()
    {
        Vector2 v = InputRouting.Instance.GetMoveInput () * moveSpeed;

        rb.velocity = new Vector3 (v.x, v.y, driftSpeed);

    }
}
