using System.Linq;
using UnityEngine;

public class PassableGroundTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("If enabled, the player can drop down by pressing a key.")]
    private bool passableWithKeyPress;
    private bool isDownKeyPressed;

    private BoxCollider2D collisionBox;
    private BoxCollider2D triggerBox;

    private Bounds collisionBoxBounds;
    private Bounds triggerBoxBounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        collisionBox = colliders.FirstOrDefault(c => !c.isTrigger);
        triggerBox = colliders.FirstOrDefault(c => c.isTrigger);

        if (!collisionBox || !triggerBox)
        {
            Debug.LogWarning((!collisionBox ? "Collision Box " + (!triggerBox ? "und Trigger Box " : "") : "Trigger Box ") + "nicht gefunden!");
            return;
        }

        collisionBoxBounds = collisionBox.bounds;
        triggerBoxBounds = triggerBox.bounds;
    }

    // Update is called once per frame
    void Update()
    {
        isDownKeyPressed = Input.GetKey(KeyCode.S);

        if (passableWithKeyPress && isDownKeyPressed)
            collisionBox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collisionBox || !triggerBox) return;

        if (!collision.CompareTag("Player") || collision is not BoxCollider2D)
            return;

        if (collision.bounds.min.y > collisionBoxBounds.max.y)
            collisionBox.enabled = true;
        else collisionBox.enabled = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collisionBox || !triggerBox) return;

        if (!collision.CompareTag("Player") || collision is not BoxCollider2D)
            return;

        if (collision.bounds.min.y > collisionBoxBounds.center.y + collisionBoxBounds.extents.y && !(passableWithKeyPress && isDownKeyPressed))
            collisionBox.enabled = true;
        else collisionBox.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collisionBox || !triggerBox) return;

        if (!collision.CompareTag("Player") || collision is not BoxCollider2D)
            return;

        collisionBox.enabled = true;
    }
}
