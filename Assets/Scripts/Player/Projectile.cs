using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool hit;
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
        if (lifeTime > 5f) //put to whatever, deactivates projectile after 5 seconds
        {
            gameObject.SetActive(false);
            lifeTime = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Ghost ghost = other.GetComponent<Ghost>();
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
    }

    public void SetDirection(float _direction)
    {
        lifeTime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        cCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
