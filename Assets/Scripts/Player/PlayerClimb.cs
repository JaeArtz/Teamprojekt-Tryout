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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        climbUp = Input.GetKey(KeyCode.W);
        climbDown = Input.GetKey(KeyCode.S);

        CanJump = canClimb && !climbUp && !climbDown && Input.GetKey(KeyCode.LeftShift);
        //Debug.Log(isClimbKeyDown ? "Climb Key is down" : "Nada");

        bool isClimbKeyDown = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
        if (canClimb && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space))
            body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        if (!Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space) || isClimbKeyDown)
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

        //else
        //    Debug.LogError("Body kann nicht climben weil " + (canClimb ? "canClimb ist false" : "isClimbKeyDown ist false"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision || !body)
            return;

        //if (collision.CompareTag("PassableGround"))
        //{
        //    Vector2 collPos = collision.transform.position;
        //    Vector2 bodyPos = body.transform.position;

        //    Vector2 distance = collPos - bodyPos;
        //    if(distance.y < 0)
        //    {
        //        canClimb = false;
        //    }
        //}

        if (!collision.CompareTag("Climbable"))
            return;

        canClimb = true;
        Debug.Log("enter");
        //body.constraints |= RigidbodyConstraints2D.FreezePositionY;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision || !body)
            return;

        if (!collision.CompareTag("Climbable"))
            return;

        canClimb = false;

        Debug.Log("exit");
        //body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }
}
