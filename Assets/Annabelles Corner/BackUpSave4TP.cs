using System;

public class Class1
{
	public Class1()
	{

    // ---------------------------------------------------ExtraStuff from Annabelle ------------------------------------------------

        // this property is for use in "BouncyMushroom"
    public float VerticalVelocity
    {
        get => body.linearVelocity.y;
        set
        {
            // only sets y-value, in a new vector, x-value stays the same
            body.linearVelocity = new Vector2(body.linearVelocity.x, value);

            // this should ensure that player has an extra jump in air, after bouncing up
            if (value > 0)
            {
                jumpCounter = canDoubleJump ? extraJumps : 0;
                groundCoyoteCounter = 0;
            }
        }
    }

    // these properties are used for "SinkingGround"
    public float MaxVelocityX { get => playerMaxVelocityX; set => playerMaxVelocityX = value; }
    public float MaxVelocityY { get => playerMaxVelocityY; set => playerMaxVelocityY = value; }

    public float remoteAccessToGroundCoyoteCounter { get => groundCoyoteCounter; set => groundCoyoteCounter = value; }
    //--------------------------------------------------------------------------------------------------------------------------------

}
}
