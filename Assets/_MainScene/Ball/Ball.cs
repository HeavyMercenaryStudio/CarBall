using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision coll)
    {
        // find collision point and normal. You may want to average over all contacts
        if (coll.collider.name == "ring")
        {
            var dir = coll.contacts[0].normal;
            rb.AddForce(dir*1000);
        }
    }
}
