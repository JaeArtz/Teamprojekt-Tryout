using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    private float speed;
    private float range;
    private Vector3 startPosition;
    
    public void setRange(float range)
    {
        this.range = range;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    public void ActivateProjectile() // Aktiviert das Projektil und setzt die Startposition
    {
        startPosition = transform.position;
        gameObject.SetActive(true);
    }

    private void ReturnToPool() // Rückgabe des Projektils an den Pool
    {
        ProjectilePool.Instance.ReturnArrowToPool(this);
    }

    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(-movementSpeed, 0, 0, Space.World); // Bewegt das Projektil nach links
        
        if (Vector3.Distance(startPosition, transform.position) >= range) // maximale Reichweite erreicht?
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        base.OnTriggerStay2D(other); // Basisklassen-Logik für Schaden anwenden
        if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("Player") || other.CompareTag("PlayerProjectile"))
        {
            ReturnToPool();
        }
    }
}