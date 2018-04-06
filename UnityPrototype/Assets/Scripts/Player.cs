using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = .2f;   //  Determines velocity.x control in air.
    float accelerationTimeGrounded = .1f;   //  Likewise for ground.
    public float moveSpeed = 6;
    float origMoveSpeed;
    public float crouchSpeed = 2.5f;
    public bool crouching;
    [HideInInspector]
    public bool crouchCanStand; // Bool variable tied to CeilingCheckCrouch. Determines if player can stand while crouching.

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;

    public bool enableWallJumpAndSlide = true;

    float gravity;

    float maxJumpVelocity;
    float minJumpVelocity;
    
    Vector2 velocity;
    float velocityXSmoothing;

    Controller2D controller;    // Controller2D class that handles player raycasts.

    Animator animator;

    Vector2 directionalInput;

    bool wallSliding = false;
    int wallDirX = 0;

    float origColliderSizeY;
    float origColliderOffsetY;
    float crouchColliderSizeY;
    float crouchColliderOffsetY;

    bool facingRight = true;

    void Start()
    {
        controller = GetComponent<Controller2D> ();
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2); // Gravity during jumping. Based on Physics equation.
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        origMoveSpeed = moveSpeed;
        origColliderOffsetY = controller.collider.offset.y;
        origColliderSizeY = controller.collider.size.y;
        crouchColliderOffsetY = -.25f;
        crouchColliderSizeY = origColliderSizeY / 2;
    }

    void FixedUpdate()
    {

        CalculateVelocity();
        HandleWallSliding();
        controller.Move(velocity * Time.deltaTime, directionalInput); // Gets input from controller to move player.
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
        if (crouchCanStand) // If the player can stand up from crouching, crouching detection reverts to default.
        {
            crouching = controller.collisions.crouching;
        }
        else // If the level blocks the player from crouching, force the player to keep crouching.
        {
            crouching = controller.collisions.crouching = true;        
        }


        if (crouching)    // If crouching...
        {
            controller.collider.size = new Vector2(controller.collider.size.x, crouchColliderSizeY);
            controller.collider.offset = new Vector2(controller.collider.offset.x, crouchColliderOffsetY);
            moveSpeed = crouchSpeed;    // Sets current speed to slowed speed during crouch.
        }
        else // Resets to standing position.
        {
            controller.collider.offset = new Vector2(controller.collider.offset.x, origColliderOffsetY);
            controller.collider.size = new Vector2(controller.collider.size.x, origColliderSizeY);
            moveSpeed = origMoveSpeed;  // Sets current speed to default speed.
        }

        animator.SetBool("Crouching", crouching); // Sets animator parameter for bool crouching to collisions crouching.;

        if (directionalInput.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (directionalInput.x < 0 && facingRight)
        {
            Flip();
        }

    }

    public void SetDirectionalInput (Vector2 input)
    {
        directionalInput = input;
    }

    public void onJumpInputDown()
    {
        if (wallSliding && enableWallJumpAndSlide)
        {
            if (wallDirX == directionalInput.x)    // Jump up on single wall.
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)  //  Jump off of wall to ground.
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else // Jump towards other wall, like Mario's wallkick.
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (controller.collisions.below && !controller.collisions.crouching)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // not jumping against max slope.
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void onJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)   // Prevents acceleration in the middle of the air.
        {
            velocity.y = minJumpVelocity;
        }
    }

    void HandleWallSliding()
    {

        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if (enableWallJumpAndSlide)
        {
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                wallSliding = true;
                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;   // Horizontal movement due to directionalInput;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);    //  Smooths horizontal movement, accounts for ground and air.
        velocity.y += gravity * Time.deltaTime; // Vertical movement due to gravity.
        
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
