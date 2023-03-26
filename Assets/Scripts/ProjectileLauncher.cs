using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;    // The prefab of the projectile we want to launch
    public Transform player;          // The point from which the projectile will be launched
    public Transform hole;                // The target that the projectile will hit
    public LayerMask hazards;               // The hazards on the grid
    private Rigidbody rigidBody;
    public float velocity;                 // The velocity of the projectile
    public float launchAngle;              // The angle at which the projectile will be launched in degrees
    private float angle;                    // The angle of the projectile in degrees

    private LineRenderer lineRenderer;     // The LineRenderer component used to draw the trajectory
    public int numberOfPoints = 100;       // The number of points to draw for the trajectory
    public float timeBetweenPoints = 0.05f;  // The time between points on the trajectory

    public float maxDistance = 10f; // The maximum distance the ball can travel in one stroke
    private float gridSize = 1f; // The size of each grid cell
    private float hazardPenalty = 10f; // The penalty for being near a hazard

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))   // Check if the space key is pressed
        {
            LaunchProjectile();
        }
    }

    private Vector3 GetLaunchVector()
    {
        // Calculate the x and y velocities of the projectile based on the given velocity, angle, and launch angle
        float xVelocity = velocity * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
        float yVelocity = velocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad);
        float zVelocity = velocity * Mathf.Sin(angle * Mathf.Deg2Rad) * Mathf.Cos(launchAngle * Mathf.Deg2Rad);

        return new Vector3(xVelocity, yVelocity, zVelocity);
    }

    private void CalculateAngleForTarget(Vector3 target)
    {
        Vector3 direction = target - player.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        this.angle = angle;
    }

    private Vector3 CalculateBestStroke()
    {
        // Calculate the grid dimensions based on the maximum distance
        int gridWidth = Mathf.RoundToInt(maxDistance / gridSize);
        int gridHeight = Mathf.RoundToInt(maxDistance / gridSize);
        int layerMask = 1 << LayerMask.NameToLayer("Hazards");

        // Generate a list of options
        List<Vector3> options = new List<Vector3>();
        for (float x = -maxDistance; x < gridWidth; x++)
        {
            for (float y = -maxDistance; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * gridSize, 0, y * gridSize);
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
            float distanceToHole = Vector3.Distance(option, hole.position);
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

    private void LaunchProjectile()
    {
        // Instantiate the projectile at the launch point
        GameObject projectileInstance = Instantiate(projectilePrefab, player.position, Quaternion.identity);
        // Get the rigidbody component of the projectile
        rigidBody = projectileInstance.GetComponent<Rigidbody>();

        Vector3 target = CalculateBestStroke();

        CalculateAngleForTarget(target);
        
        // Apply the calculated velocities to the rigidbody
        rigidBody.velocity = GetLaunchVector();

        // Apply gravity to the projectile
        rigidBody.AddForce(Vector3.down, ForceMode.Acceleration);
        
        DrawTrajectory();
    }


    private void DrawTrajectory()
    {
         // Get the LineRenderer component and set its position count
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat;

        Vector3[] positions = new Vector3[numberOfPoints];

        float timeInterval = timeBetweenPoints;
        Vector3 currentVelocity = GetLaunchVector();
        Vector3 currentPosition = player.position;
        for (int i = 0; i < numberOfPoints; i++)
        {
            positions[i] = currentPosition;

            currentPosition += currentVelocity * timeInterval + 0.5f * Physics.gravity * timeInterval * timeInterval;
            currentVelocity += Physics.gravity * timeInterval;
        }

        lineRenderer.SetPositions(positions);
    }
}
