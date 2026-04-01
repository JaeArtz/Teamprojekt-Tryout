using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] public float speed;
    public int startPoint;
    public Transform[] points; //Array der Wegpunkte

    private int i; //aktuelles Ziel Index
    void Start()
    {
        transform.position = points[startPoint].position;
        i = startPoint;
    }
    void Update()
    {
        // Prüft, ob die Plattform nah genug am aktuellen Zielpunkt angekommen ist
        if (Vector3.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; // Ziel-Index erhöhen -> Nächster Punkt im Array
            if (i == points.Length) // Wenn das Ende des Arrays erreicht ist, fange wieder bei 0 an (Loop)
            {
                i = 0;
            }
        }

        // Bewegt die Plattform gleichmäßig von der aktuellen Position zum Zielpunkt
        transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Setzt die Plattform als "Parent" des kollidierenden Objekts.
        // Dadurch bewegt sich der Spieler automatisch mit der Plattform mit.
        collision.transform.SetParent(transform);
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Entfernt die Plattform als Parent (setzt Parent auf null).
        // Der Spieler ist nun wieder unabhängig von der Plattform-Bewegung.
        collision.transform.SetParent(null);
    }

    // Zeichnet Linien im Scene-Fenster von Unity, um den Pfad sichtbar zu machen
    private void OnDrawGizmos()
    {
        if (points != null && points.Length > 1)
        {
            Gizmos.color = Color.red;
            for (int j = 0; j < points.Length - 1; j++)
            {
                Gizmos.DrawLine(points[j].position, points[j + 1].position);
            }
        }
    }
}
