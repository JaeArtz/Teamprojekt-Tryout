using UnityEngine;

public class GhostLight_PathPoints_SmoothMovement : MonoBehaviour
{
  
    [Header("WayPoint")]
    public Transform[] points;

    [Header("Movement")]
    [Tooltip("Maximum Speed of Light (or moving object).")]
    public float MovementSpeed = 2f;

    [Tooltip("How strongly should the movement be smoothed out in curves? small number = fast reaction, higher number = smoother, slower reaction.")]
    public float ReactionSmoothness = 0.2f;

    [Tooltip("Starting at which distance should the moving object start thinking about leaning into the curve towards the next point?.")]
    public float cornerRadius = 0.3f;

    [Header("Loop or PingPong?")]
    public bool loop = true;      // circular movement
    public bool pingPong = false; // back and forth (patroling)

    private Vector3[] cachedPoints;
    private int currentIndex = 0;
    private int direction = 1; // direction of Movement: 1 = moving forward, -1 = moving backward
    private Vector3 velocity;  // for SmoothDamp

    void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("GhostLightPath: No WayPoints set! You need to set a number of Points and assign objects as Points for coordinates.", this);
            enabled = false;
            return;
        }

        if (points.Length == 1)
        {
            // only one point = no path
            transform.position = points[0].position;
            enabled = false;
            return;
        }

        // taking fixed coordinates at start, so they can't change
        cachedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            cachedPoints[i] = points[i].position;

        // setting start to Point Nr 1
        transform.position = cachedPoints[0];
    }

    void Update()
    {
        if (cachedPoints == null || cachedPoints.Length == 0)
            return;

        Vector3 targetPos = cachedPoints[currentIndex];

        // smoother movement (taking in reactionSmoothness)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            ReactionSmoothness,
            MovementSpeed,
            Time.deltaTime
        );

        float dist = Vector3.Distance(transform.position, targetPos);

        // looking around the corner: starting early to "lean into the curve"
        if (dist <= cornerRadius)
        {
            // PingPong ignores Loop
            if (pingPong)
            {
                // st start: moving forward
                if (currentIndex == 0)
                    direction = 1;
                // at end point: moving backward (opposite direction along path)
                else if (currentIndex == cachedPoints.Length - 1)
                    direction = -1;

                currentIndex += direction;
            }
            else if (loop)
            {
                currentIndex++;

                if (currentIndex >= cachedPoints.Length)
                    currentIndex = 0;
            }
            else
            {
                currentIndex++;

                if (currentIndex >= cachedPoints.Length)
                {
                    enabled = false; 
                    return;
                }
            }
        }
    }
}

