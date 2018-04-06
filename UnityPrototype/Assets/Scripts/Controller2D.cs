using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController {

    public float maxSlopeAngle = 75; // Maximum angle player can walk on.
    float crouchRayFactor;

    public CollisionInfo collisions;

    [HideInInspector]
    public Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
        crouchRayFactor = 1;
    }

    void FixedUpdate()
    {
        if (collisions.crouching)
        {
            crouchRayFactor = 0.5f;
        }    
        else
        {
            crouchRayFactor = 1;
        }
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        collisions.moveAmountOld = moveAmount;
        playerInput = input;
              
        if (moveAmount.y < 0)
        {
            descendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)    // Determines which direction player is facing.
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        // Uncomment for wallsliding.
        //if (moveAmount.x != 0)    // Only need to check for collisions if there is horizontal movement.
        //{
        //    HorizontalCollisions(ref moveAmount);
        //}

        HorizontalCollisions(ref moveAmount);
        VerticalCollisions(ref moveAmount);

     
        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    // Determines what happens if player collides with something horizontally.
    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir; // Determines if player is moving up or down.
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)  //slides against wall even if you are still.
        {
           rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount * crouchRayFactor; ++i)  // crouchRayFactor adjusts how many rays are spawned to adjust for crouching.
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // Changes starting position of ray based on vertical movement.
            rayOrigin += (Vector2.up * (horizontalRaySpacing * i));  // Stacks the ray positions.
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            // Draws the ray. Note that this is purely for testing purposes.
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)    // If raycast hits something.
            {
                    if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); // If we hit a slope, calcuate its angle.

                if (i == 0 && slopeAngle <= maxSlopeAngle)  // For the bottommost ray, and if the slopeAngle is climbable.
                {
                    if (collisions.descendingSlope) // Makes sure if two slopes intersect, player isn't slowed down by disabling descendSlope physics.
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0; // Measures distance between hitting slope and side of player. (Ray distance).
                    if (slopeAngle != collisions.slopeAngleOld) // If we are climbing a new slope.
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;    // Once ClimbSlope activates, only uses velocity x when player reaches slope.
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);   // Method for climbing slopes.
                    moveAmount.x += distanceToSlopeStart * directionX;    // Readds velocity once we are finished climbing slope.
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)    // if we are not climbing a slope, or if the current slopeAngle is greater than maxSlopeAngle.
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance; //    Makes sure that if the player hits an object initially with ray, it won't move down if another ray hits a lower object.    

                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);  // Updates velocity.y if we collide with an object on slope.
                    }
                }

                collisions.left = directionX == -1; // If we hit something and moving left, collisions.left = true.
                collisions.right = directionX == 1;
            }
        }
    }

    // Determines what happens if player collides with something vertically.
    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y); // Determines if player is moving up or down.
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;    // Stacks the ray positions.
        for (int i = 0; i < verticalRayCount; ++i)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; // Changes starting position of ray based on vertical movement.
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            // Draws the ray. Note that this is purely for testing purposes.
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)    // If raycast hits something.
            {
                if (hit.collider.tag == "Passable")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .25f);
                        continue;
                    }
                }
                else if (hit.collider.tag != "Passable")
                {
                    if (playerInput.y < -0.3f)
                    {                        
                        collisions.crouching = true;
                    }
                }
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance; //    Makes sure that if the player hits an object initially with ray, it won't move down if another ray hits a lower object.    

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);    // Accounts for if player hits the bottom of an obstacle on a slope.
                }              
                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }
        }

        // Accounts for curved slope.
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x); 
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);  // Checks if we hit something AS player climbs slope.

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
                if (playerInput.y < -0.3f)
                {
                    collisions.crouching = true;
                }
            }
        }
    }

    // Takes slope and determines the velocity the player moevs up the slope through trignometry.
    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x); // Speed we want to move.
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance; 

        if (moveAmount.y <= climbVelocityY)   // If player is not jumping.
        {
            if (playerInput.y < -0.3f)
            {                
                collisions.crouching = true;
            }
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            moveAmount.y = climbVelocityY;
            collisions.below = true;    // Assume we are grounded while walking on a slope.
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
        
    }

    void descendSlope(ref Vector2 moveAmount)
    { 
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }
        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft; // Changes rayOrigin if we are moving down right or left. (Change to either bottomLeft or bottomRight).
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);   // hit.normal is a direction perpendicular to the slope.
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX) // If player is moving down slope.
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))  // If we are close enough to the slope to move down it.
                        {
                            // Following code is same as climbSlope, but adjusted for descending.
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }
    
    void ResetFallingThroughPlatform()  // Resets falling through platform condition.
    {
        collisions.fallingThroughPlatform = false;
    }

    // Similar to checking if player is on ground, but for all four collisions.
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld; // slopeAngleOld is slopeAngle from previous frame. 
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public bool crouching;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            crouching = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
