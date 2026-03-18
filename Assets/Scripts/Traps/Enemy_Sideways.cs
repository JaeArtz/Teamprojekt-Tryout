using UnityEngine;

public class Enemy_Sideways : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    [SerializeField] private Transform currentTarget;

    void Start()
    {
        // Falls nichts zugewiesen wurde, starte bei Punkt B
        if (currentTarget == null) currentTarget = pointB;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        // Distanz prüfen
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.02f)
        {
            // Ziel wechseln: Wenn B war, gehe zu A sonst zu B.
            currentTarget = (currentTarget == pointB) ? pointA : pointB;
        }

        // Bewegung
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
    }

    // Lininen fürs Level Design, kann nahcher weg
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}