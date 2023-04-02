using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;    // The prefab of the projectile we want to launch

    public GameObject currentTarget;
    public float velocity;                 // The velocity of the projectile
    public float launchAngle;              // The angle at which the projectile will be launched in degrees
    private LineRenderer lineRenderer;     // The LineRenderer component used to draw the trajectory
    public int numberOfPoints = 100;       // The number of points to draw for the trajectory
    public float timeBetweenPoints = 0.05f;  // The time between points on the trajectory
    public float maxDistance = 10f; // The maximum distance the ball can travel in one stroke
    private float gridSize = 1f; // The size of each grid cell
    private float hazardPenalty = 10f; // The penalty for being near a hazard
    private bool isProjectileReady = true;

    public bool autoShot = false;

    private void Start()
    {
        if (autoShot)
        {

            LaunchProjectile();
        }
    }

    private void Update()
    {
        if (!autoShot && Input.GetKeyDown(KeyCode.Space))   // Check if the space key is pressed
        {
            LaunchProjectile();
        }
    }

    private void LaunchProjectile()
    {
        if (isProjectileReady)
        {

            GameObject projectileInstance = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);

            Vector3 target = CalculateBestStroke();
            float angle = CalculateAngleForTarget(target);
            float velocity = CalculateProjectileVelocity(gameObject.transform.position, target, this.launchAngle);
            Debug.Log("velocity: " + velocity);
            Vector3 launchVelocity = GetLaunchVector(angle, velocity);

            projectileInstance.GetComponent<Projectile>().launchDelegate(launchVelocity, HandleResult);
            isProjectileReady = false;

            // Line renderer component makes shit weird, add as a child or something
            // DrawTrajectory(launchVelocity);
        }
    }

    void HandleResult(Vector3 finalPosition)
    {
        Vector3 newPosition = new Vector3(finalPosition.x, gameObject.transform.position.y, finalPosition.z);
        gameObject.transform.position = newPosition;
        isProjectileReady = true;
        if (autoShot)
        {
            LaunchProjectile();
        }
    }

    private float CalculateProjectileVelocity(Vector3 originalPosition, Vector3 targetPosition, float launchAngle)
    {
        // Calculate the distance to the target
        float distance = Vector3.Distance(originalPosition, targetPosition);

        // Calculate the initial velocity required to hit the target
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * launchAngle * Mathf.Deg2Rad));

        return velocity;
    }

    private Vector3 GetLaunchVector(float angle, float velocity)
    {
        // Calculate the x and y velocities of the projectile based on the given velocity, angle, and launch angle
        float xVelocity = velocity * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
        float yVelocity = velocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad);
        float zVelocity = velocity * Mathf.Sin(angle * Mathf.Deg2Rad) * Mathf.Cos(launchAngle * Mathf.Deg2Rad);

        return new Vector3(xVelocity, yVelocity, zVelocity);
    }

    private float CalculateAngleForTarget(Vector3 target)
    {
        Vector3 direction = target - gameObject.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        return angle;
    }

    private Vector3 CalculateBestStroke()
    {
        // Calculate the grid dimensions based on the maximum distance
        float playerPositionX = Mathf.Round(gameObject.transform.position.x);
        float playerPosistionZ = Mathf.Round(gameObject.transform.position.z);
        int gridWidth = Mathf.RoundToInt(maxDistance / gridSize);
        int gridHeight = Mathf.RoundToInt(maxDistance / gridSize);
        int layerMask = 1 << LayerMask.NameToLayer("Hazards");


        // Generate a list of options
        List<Vector3> options = new List<Vector3>();
        for (float x = playerPositionX - maxDistance; x < playerPositionX + gridWidth; x++)
        {
            for (float z = playerPosistionZ - maxDistance; z < playerPosistionZ + gridHeight; z++)
            {
                Vector3 position = new Vector3(x * gridSize, 0, z * gridSize);
                if (!Physics.CheckSphere(position, gridSize / 2f, layerMask))
                {
                    options.Add(position);
                }
            }
        }

        // Score each option
        Dictionary<Vector3, float> scores = new Dictionary<Vector3, float>();
        foreach (Vector3 option in options)
        {
            float distanceToHole = Vector3.Distance(option, currentTarget.transform.position);
            float score = 1f / distanceToHole; // Score based on distance to hole
            if (Physics.CheckSphere(option, gridSize, layerMask))
            {
                score /= hazardPenalty; // Apply a penalty for being near a hazard
            }
            scores.Add(option, score);
        }

        // Choose the best option
        Vector3 bestOption = Vector3.zero;
        float bestScore = float.MinValue;
        foreach (KeyValuePair<Vector3, float> pair in scores)
        {
            if (pair.Value > bestScore)
            {
                bestOption = pair.Key;
                bestScore = pair.Value;
            }
        }
        // Move the ball to the best option
        Debug.Log("best option: " + bestOption);

        return bestOption;
    }




    // private void DrawTrajectory(Vector3 launchVelocity)
    // {
    //     // Get the LineRenderer component and set its position count
    //     lineRenderer = GetComponent<LineRenderer>();
    //     lineRenderer.positionCount = numberOfPoints;
    //     lineRenderer.startColor = Color.white;
    //     lineRenderer.endColor = Color.white;
    //     lineRenderer.startWidth = 0.04f;
    //     lineRenderer.endWidth = 0.04f;
    //     Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
    //     lineRenderer.material = whiteDiffuseMat;

    //     Vector3[] positions = new Vector3[numberOfPoints];

    //     float timeInterval = timeBetweenPoints;
    //     Vector3 currentVelocity = launchVelocity;
    //     Vector3 currentPosition = gameObject.transform.position;
    //     for (int i = 0; i < numberOfPoints; i++)
    //     {
    //         positions[i] = currentPosition;

    //         currentPosition += currentVelocity * timeInterval + 0.5f * Physics.gravity * timeInterval * timeInterval;
    //         currentVelocity += Physics.gravity * timeInterval;
    //     }

    //     lineRenderer.SetPositions(positions);
    // }
}
