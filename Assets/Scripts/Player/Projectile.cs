using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Light2D ballLight;
    private bool hit; // Projektil was getroffen?
    private CircleCollider2D cCollider;
    private float direction;

    private float lifeTime;

    private void Awake()
    {
        cCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
        lifeTime += Time.deltaTime;

        if (lifeTime > 5f) //Projektil nach 5 Sekuden entfernen wenn nichts getroffen wird
        {
            gameObject.SetActive(false);
            lifeTime = 0;
        }
    }

    // Wenn projektil was trifft, wird es deaktiviert und zurück in den Pool gegeben
    void OnTriggerEnter2D(Collider2D other)
    {
        Ghost2 ghost = other.GetComponent<Ghost2>();
        if(ghost != null)
        {
            ghost.HitByLight();
            hit = true;
            cCollider.enabled = false;
            Deactivate();
            return;
        }

        hit = true;
        cCollider.enabled = false;
        Deactivate();

        Debug.Log("Projectile hit: " + other.name);
    }

    // Um Schuss zu initialisieren, Richtung setzen und aktivieren
    public void SetDirection(float _direction)
    {
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        cCollider.enabled = true;

        // Sprite an Flugrichtung anpassen
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }


        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        ProjectilePool.Instance.ReturnToPool(this);
    }

    // Zurücksetzen des Projektils, bevor es wiederverwendet wird
    public void ResetProjectile()
    {
        hit = false;
        lifeTime = 0f;
        cCollider.enabled = true;
        if(ballLight != null)
        {
            ballLight.enabled = true;
        }
    }
}
