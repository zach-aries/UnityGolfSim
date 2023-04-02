using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void HandleLaunchReady(Vector3 finalPosition);
    private HandleLaunchReady handleLaunchReady;
    public delegate void LaunchDelegate(Vector3 launchVelocity, HandleLaunchReady handleLaunchReady);
    public LaunchDelegate launchDelegate;

    private Rigidbody rigidBody;
    public float friction = 0.5f;
    public float stopSpeed = 1f;
    private Vector3 launchVelocity;   // Initial launch vector of projectile


    void Awake()
    {
        //gameObject.layer uses only integers, but we can turn a layer name into a layer integer using LayerMask.NameToLayer()
        int layerIndex = LayerMask.NameToLayer("GolfBall");
        gameObject.layer = layerIndex;
        launchDelegate = HandleLaunch;
    }

    public void HandleLaunch(Vector3 launchVelocity, HandleLaunchReady handleLaunchReady) // 
    {
        rigidBody = GetComponent<Rigidbody>();

        // rigidBody.velocity = launchVelocity;
        rigidBody.AddForce(launchVelocity.x, launchVelocity.y, launchVelocity.z, ForceMode.VelocityChange);
        // rigidBody.AddRelativeForce(launchVelocity); //.AddRelativeForce(new Vector3(0, launchVelocity,0));

        this.launchVelocity = launchVelocity;
        // Call the handleLaunchReady method with the result
        if (handleLaunchReady != null)
        {
            this.handleLaunchReady = handleLaunchReady;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (rigidBody != null && collision.gameObject.tag == "Plane")
        {
            // Apply a force opposite to the plane's normal vector
            Vector3 force = -collision.contacts[0].normal * rigidBody.velocity.magnitude * friction;
            rigidBody.AddForce(force);

            // Stop the sphere if its speed falls below the stopSpeed threshold
            if (rigidBody.velocity.magnitude < stopSpeed)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                Destroy(gameObject);
                this.handleLaunchReady(gameObject.transform.position);
            }
        }
    }
}
