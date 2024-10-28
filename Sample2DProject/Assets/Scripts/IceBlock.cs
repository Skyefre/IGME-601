using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class IceBlock : ProjectileBehavior
{
    //variables (commented out ones aren't necessary right now but I believe we'll need them for
    //public float hspd = 0.0f;
    //public float vspd = 0.0f;
    //public float lifeTime = 5.0f;
    //public GameObject owner;
    //public bool facingRight = true;
    public GameObject hurtbox;
    ////public int damage = 1;
    //public int xoffset;
    //public int yoffset;
    //public int width;
    //public int height;
    //public int xKnockback;
    //public int yKnockback;
    //public int hitstun;
    public int gravity = 1;
    public int maxHspd = 10;
    public int maxVspd = 10;
    public int lerpDelay = 0;

    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public RaycastHit2D grounded;
    public RaycastHit2D collidedCeiling;
    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component

    //so that the block only persists a certain amount of time
    //protected float timer = 0.0f;
    private void Awake()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        grounded = IsGrounded();
        collidedCeiling = IsTouchingCeiling();

        //check for ground collision
        if (grounded.collider != null)
        {
            SnapToSurface(grounded);
            vspd = 0;
        }
        //check horizontal collision
        RaycastHit2D hitWallRay = IsTouchingWall();
        if (hitWallRay.collider != null && hitWallRay.collider.gameObject.tag != "slope")
        {
            if (hitWallRay.point.x < gameObject.transform.position.x)
            {
                if (hspd < 0)
                {
                    hspd = 0;
                }
            }
            else
            {
                if (hspd > 0)
                {
                    hspd = 0;
                }
            }
        }
        // check for ceiling
        collidedCeiling = IsTouchingCeiling();
        if (collidedCeiling.collider != null)
        {
            vspd = vspd > 0 ? 0 : vspd;
            SnapToCeiling(collidedCeiling);
        }

        gameObject.transform.position += new Vector3(hspd, vspd, 0);
        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;

        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            DestroyProjectile();
        }
    }

    //destroy the block
    public override void DestroyProjectile()
    {
        timer = 0;
        //hitbox.GetComponent<Hitbox>().hitboxActive = false;
        gameObject.SetActive(false);
    }

    //spawn a block in front of the player
    public override void InitProjectile(int spawnX, int spawnY)
    {
        if (owner == null)
        {
            Debug.Log("Block owner not set");
            return;
        }
        hspd = 0;
        vspd = 0;
        facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : (owner.GetComponent<Enemy>() != null ? owner.GetComponent<Enemy>().facingRight : true);

        gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
    }

    #region Collision Detection

    private RaycastHit2D IsGrounded()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = -(vspd - 5);

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.min.y -1);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.min.y-1);

        // Cast rays downwards
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D nullHit = new RaycastHit2D();

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        // Return the point of collision if either ray hits the ground
        if (leftHit.collider != null && rightHit.collider != null)
        {
            if (leftHit.point.y > rightHit.point.y)
            {
                return leftHit;
            }
            else if (leftHit.point.y < rightHit.point.y)
            {
                return rightHit;
            }
            else
            {
                return facingRight ? rightHit : leftHit;
            }
        }
        if (leftHit.collider != null)
        {
            return leftHit;
        }
        else if (rightHit.collider != null)
        {
            return rightHit;
        }
        return nullHit;
    }

    private RaycastHit2D IsTouchingWall()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = hspd;

        // Calculate the positions for the top and bottom rays on the left and right sides
        Vector2 topLeftRayOrigin = new Vector2(bounds.min.x -1, bounds.max.y - rayOffset.y);
        Vector2 bottomLeftRayOrigin = new Vector2(bounds.min.x -1, bounds.min.y + rayOffset.y);
        Vector2 centerLeftRayOrigin = new Vector2(bounds.min.x - 1, bounds.center.y);
        Vector2 topRightRayOrigin = new Vector2(bounds.max.x +1, bounds.max.y - rayOffset.y);
        Vector2 bottomRightRayOrigin = new Vector2(bounds.max.x +1, bounds.min.y + rayOffset.y);
        Vector2 centerRightRayOrigin = new Vector2(bounds.max.x + 1, bounds.center.y);


        // Cast rays to the left and right
        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D centerLeftHit = Physics2D.Raycast(centerLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D topRightHit = Physics2D.Raycast(topRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D bottomRightHit = Physics2D.Raycast(bottomRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D centerRightHit = Physics2D.Raycast(centerRightRayOrigin, Vector2.right, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(topLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(bottomLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(centerLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(topRightRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(bottomRightRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(centerRightRayOrigin, Vector2.right * rayLength, Color.blue);

        // Return true if any of the rays hit a wall
        if (topLeftHit.collider != null)
        {
            return topLeftHit;
        }
        else if (bottomLeftHit.collider != null)
        {
            return bottomLeftHit;
        }
        else if (centerLeftHit.collider != null)
        {
            return centerLeftHit;
        }
        else if (topRightHit.collider != null)
        {
            return topRightHit;
        }
        else if (bottomRightHit.collider != null)
        {
            return bottomRightHit;
        }
        else if (centerRightHit.collider != null)
        {
            return centerRightHit;
        }
        else
        {
            return new RaycastHit2D();
        }
    }

    private RaycastHit2D IsTouchingCeiling()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = vspd;

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.max.y+1);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.max.y + 1);

        // Cast rays upwards
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.up, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.up, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.up * rayLength, Color.green);
        Debug.DrawRay(rightRayOrigin, Vector2.up * rayLength, Color.green);

        if (leftHit.collider != null)
        {
            return leftHit;
        }
        else if (rightHit.collider != null)
        {
            return rightHit;
        }
        else
        {
            return new RaycastHit2D();
        }
    }

    public float getColliderSurface(float xValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.min.y - 1); // Adjust the y value as needed

        // Cast a ray downwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.down * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.y;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public float getColliderCeiling(float xValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.min.y + 1); // Adjust the y value as needed

        // Cast a ray upwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.up * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.y;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public void SnapToSurface(RaycastHit2D hitRay)
    {
        float surfaceYVal = getColliderSurface(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, surfaceYVal, 0);
    }

    public void SnapToCeiling(RaycastHit2D hitRay)
    {
        float ceilingYVal = getColliderCeiling(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, ceilingYVal - boxCollider.bounds.size.y - 1, 0);
    }
    #endregion

    public void LerpHspd(int targetHspd, int lerpval)
    {
        if (lerpDelay >= lerpval)
        {
            lerpDelay = 0;
            if (hspd < targetHspd)
            {
                hspd++;
            }
            else if (hspd > targetHspd)
            {
                hspd--;
            }
        }
        else
        {
            lerpDelay++;
        }

        return;
    }
}
