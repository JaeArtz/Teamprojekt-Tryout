using UnityEngine;

public class GhostLight_PathPoint : MonoBehaviour
{

    public Transform[] points;
    public float MovementSpeed = 2f;
    public bool loop = true;      // circular movement
    public bool pingPong = false; // pingpong movement

    private Vector3[] cachedPoints;
    private int currentIndex = 0;
    private int direction = 1; // movement direction,forward/ backward

    void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("GhostLightPath: No WayPoints set!", this);
            enabled = false;
            return;
        }

        // saving fixed coordinates for Points at start
        cachedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            cachedPoints[i] = points[i].position;
        }

        transform.position = cachedPoints[0];
    }

    void Update()
    {
        if (cachedPoints == null || cachedPoints.Length == 0) return;

        Vector3 targetPos = cachedPoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            MovementSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            // if pingpong active: ignore loop
            if (pingPong && cachedPoints.Length > 1)
            {
                // default at start is "moving forward"
                if (currentIndex == 0)
                {
                    direction = 1;
                }
                // at end point turn around, move backwards back to startpoint
                else if (currentIndex == cachedPoints.Length - 1)
                {
                    direction = -1;
                }

                currentIndex += direction;
            }
            // else: loop behavior of object (if loop is set)...
            else if (loop)
            {
                currentIndex++;

                if (currentIndex >= cachedPoints.Length)
                {
                    currentIndex = 0;
                }
            }
            // otherwise (move once? and) stop
            else
            {
                currentIndex++;

                if (currentIndex >= cachedPoints.Length)
                {
                    enabled = false;
                }
            }
        }
    }
}






/*
 Steps for using it to create a path:

-attach script to light object (can also be an "animated" flickering light)
-set array to number of points, or drag in
-chose between circular movement and pingpong => pingpong = back and forth (patrol between points)
-You should be able to create empty objects, and set them as points (Inspector)
-loop enables moving around in a circle:
From Point 1 -> to Point 2 -> to Point 3 -> to Point 1 ....
-pingpong enables "patroling" behavior:
From Point 1 -> to Point 2 -> to Point 3 -> to Point 2 -> to Point 1 ....
-in Inspector: only set EITHER loop, OR pingpong (set both at your own risk...

 */
