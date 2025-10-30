using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //Time player can still jump after leaving ground
    private float coyoteCounter; //how much time since leaving ground or object

    [Header("Multi Jump")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && (coyoteCounter > 0 || jumpCounter > 0))
        {
            Jump();
        }

        if (isGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpCounter = extraJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime; //Start decrease counter
        }

    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0)
        {
            return; //No jump if coyote time is over
        }

        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        }
        else
        {
            if (coyoteCounter > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            }
            else
            {
                if(jumpCounter > 0)
                {
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
                    jumpCounter--;
                }
            }
        }

        coyoteCounter = 0; //Reset coyote counter after jump
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size, 0, new Vector2(transform.localScale.x , 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
