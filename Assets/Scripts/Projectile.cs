using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float friction = 0.5f;
    public float stopSpeed = 1f;

    private Rigidbody rb;

    void Awake()
    {
        //gameObject.layer uses only integers, but we can turn a layer name into a layer integer using LayerMask.NameToLayer()
        int layerIndex = LayerMask.NameToLayer("GolfBall");
        gameObject.layer = layerIndex;
    }

    void Start () {
        rb = GetComponent<Rigidbody> ();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
        {
           // Apply a force opposite to the plane's normal vector
            Vector3 force = -collision.contacts[0].normal * rb.velocity.magnitude * friction;
            rb.AddForce(force);

            // Stop the sphere if its speed falls below the stopSpeed threshold
            if (rb.velocity.magnitude < stopSpeed)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
