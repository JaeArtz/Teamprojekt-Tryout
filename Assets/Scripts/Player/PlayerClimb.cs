using System;
using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    [Header("Ladder Movement")]
    [SerializeField, Tooltip("The speed with which the player can climb upwards")]
    private float climbingSpeed = 5;
    [SerializeField, Tooltip("The speed with which the player can climb downwards")]
    private float climbingSpeedDown = -5;

    private Rigidbody2D body;
    public Rigidbody2D Body { set { body = value; } }

    private bool canClimb = false;
    public bool CanJump { get { return canClimb; } private set {} }

    private bool climbUp = false;
    private bool climbDown = false;
    private bool isOnTop = false;

    public bool IsClimbing => canClimb && (climbUp ^ climbDown);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        climbUp = Input.GetKey(KeyCode.W);
        climbDown = Input.GetKey(KeyCode.S);

        CanJump = canClimb && !climbUp && !climbDown && Input.GetKey(KeyCode.LeftShift);

        bool isClimbKeyDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
        if (canClimb && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space))
            body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        if (!canClimb || !Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space) || isClimbKeyDown)
            body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!body)
            return;

        if (!canClimb)
            return;
        
        if (climbUp & !climbDown)
            body.linearVelocityY = climbingSpeed;
        if (climbDown & !climbUp)
            body.linearVelocityY = climbingSpeedDown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision || !body)
            return;

        if (!collision.CompareTag("Climbable"))
            return;

        canClimb = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision || !body)
            return;

        if (!collision.CompareTag("Climbable"))
            return;

        canClimb = false;
    }
}
