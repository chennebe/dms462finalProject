using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;

    public const float skinWidth = .015f; // Width raycasts are embedded in within player.
    const float dstBetweenRays = .2f;

    [HideInInspector]
    public int horizontalRayCount;  // Amount of rays horizontally.
    [HideInInspector]
    public int verticalRayCount;    // Amount of rays vertically.

    [HideInInspector]
    public float horizontalRaySpacing; // Spacing between horizontal rays.
    [HideInInspector]
    public float verticalRaySpacing;   // Spacing between vertical rays.

    [HideInInspector]
    public BoxCollider2D collider; // Player boxCollider2D.
    public RaycastOrigins raycastOrigins; // Reference to raycast origins.

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;    // Bounds of the boxCollider. (Basically it's outline).
        bounds.Expand(skinWidth * -2); // Readjusts with skinWidth.

        // Sets raycastOrigins to positions around the player.
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        // Calculates spacing betweens rays. (bounds / sections by rays).
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    // Stores positions of raycast origin locations.
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
