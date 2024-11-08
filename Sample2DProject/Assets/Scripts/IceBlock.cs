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
        else
        {
            vspd -= gravity;
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

        //destroy projectile after lifetime
        if (disableAfterAnimation)
        {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                DestroyProjectile();
            }
        }
        else
        {
            //destroy projectile after lifetime
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                DestroyProjectile();
            }
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
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.min.y +1);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.min.y+1);

        // Cast rays downwards
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(rightRayOrigin, Vector2.down, rayLength, groundLayer);

        RaycastHit2D leftHit;
        RaycastHit2D rightHit;

        // Check if the ray hit a collider
        if (leftHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (leftHits[0].collider.gameObject == gameObject)
            {
                if(leftHits.Length > 1)
                {
                    leftHit = leftHits[1];
                }
                else
                {
                    leftHit = new RaycastHit2D();
                }
            }
            else
            {
                leftHit = leftHits[0];
            }
        }
        else
        {
            leftHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (rightHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (rightHits[0].collider.gameObject == gameObject)
            {
                if (rightHits.Length > 1)
                {
                    rightHit = rightHits[1];
                }
                else
                {
                    rightHit = new RaycastHit2D();
                }
            }
            else
            {
                rightHit = rightHits[0];
            }
        }
        else
        {
            rightHit = new RaycastHit2D();
        }

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
        Vector2 topLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.max.y - rayOffset.y);
        Vector2 bottomLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.min.y + rayOffset.y);
        Vector2 centerLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.center.y);
        Vector2 topRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.max.y - rayOffset.y);
        Vector2 bottomRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.min.y + rayOffset.y);
        Vector2 centerRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.center.y);


        // Cast rays to the left and right
        RaycastHit2D[] topLeftHits = Physics2D.RaycastAll(topLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D[] bottomLeftHits = Physics2D.RaycastAll(bottomLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D[]centerLeftHits = Physics2D.RaycastAll(centerLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D[] topRightHits = Physics2D.RaycastAll(topRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D[] bottomRightHits = Physics2D.RaycastAll(bottomRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D[] centerRightHits = Physics2D.RaycastAll(centerRightRayOrigin, Vector2.right, rayLength, groundLayer);

        RaycastHit2D topLeftHit; 
        RaycastHit2D bottomLeftHit;
        RaycastHit2D centerLeftHit;
        RaycastHit2D topRightHit;
        RaycastHit2D bottomRightHit;
        RaycastHit2D centerRightHit;

        // Check if the ray hit a collider
        if (topLeftHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (topLeftHits[0].collider.gameObject == gameObject)
            {
                if (topLeftHits.Length > 1)
                {
                    topLeftHit = topLeftHits[1];
                }
                else
                {
                    topLeftHit = new RaycastHit2D();
                }

            }
            else
            {
                topLeftHit = topLeftHits[0];
            }
        }
        else
        {
            topLeftHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (bottomLeftHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (bottomLeftHits[0].collider.gameObject == gameObject)
            {
                if (bottomLeftHits.Length > 1)
                {
                    bottomLeftHit = bottomLeftHits[1];
                }
                else
                {
                    bottomLeftHit = new RaycastHit2D();
                }
            }
            else
            {
                bottomLeftHit = bottomLeftHits[0];
            }
        }
        else
        {
            bottomLeftHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (centerLeftHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (centerLeftHits[0].collider.gameObject == gameObject)
            {
                if(centerLeftHits.Length > 1)
                {
                    centerLeftHit = centerLeftHits[1];
                }
                else
                {
                    centerLeftHit = new RaycastHit2D();
                }
            }
            else
            {
                centerLeftHit = centerLeftHits[0];
            }
        }
        else
        {
            centerLeftHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (topRightHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (topRightHits[0].collider.gameObject == gameObject)
            {
                if (topRightHits.Length > 1)
                {
                    topRightHit = topRightHits[1];
                }
                else
                {
                    topRightHit = new RaycastHit2D();
                }
            }
            else
            {
                topRightHit = topRightHits[0];
            }
        }
        else
        {
            topRightHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (bottomRightHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (bottomRightHits[0].collider.gameObject == gameObject)
            {
                if (bottomRightHits.Length > 1)
                {
                    bottomRightHit = bottomRightHits[1];
                }
                else
                {
                    bottomRightHit = new RaycastHit2D();
                }
            }
            else
            {
                bottomRightHit = bottomRightHits[0];
            }
        }
        else
        {
            bottomRightHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (centerRightHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (centerRightHits[0].collider.gameObject == gameObject)
            {
                if (centerRightHits.Length > 1)
                {
                    centerRightHit = centerRightHits[1];
                }
                else
                {
                    centerRightHit = new RaycastHit2D();
                }
            }
            else
            {
                centerRightHit = centerRightHits[0];
            }
        }
        else
        {
            centerRightHit = new RaycastHit2D();
        }

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
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.max.y-1);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.max.y-1);

        // Cast rays upwards
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(leftRayOrigin, Vector2.up, rayLength, groundLayer);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(rightRayOrigin, Vector2.up, rayLength, groundLayer);

        RaycastHit2D leftHit;
        RaycastHit2D rightHit;

        // Check if the ray hit a collider
        if (leftHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (leftHits[0].collider.gameObject == gameObject)
            {
                if (leftHits.Length > 1)
                {
                    leftHit = leftHits[1];
                }
                else
                {
                    leftHit = new RaycastHit2D();
                }
            }
            else
            {
                leftHit = leftHits[0];
            }
        }
        else
        {
            leftHit = new RaycastHit2D();
        }

        // Check if the ray hit a collider
        if (rightHits.Length > 0)
        {
            //check if the first hit is the block itself
            if (rightHits[0].collider.gameObject == gameObject)
            {
                if (rightHits.Length > 1)
                {
                    rightHit = rightHits[1];
                }
                else
                {
                    rightHit = new RaycastHit2D();
                }
            }
            else
            {
                rightHit = rightHits[0];
            }
        }
        else
        {
            rightHit = new RaycastHit2D();
        }

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
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.max.y - 1); // Adjust the y value as needed

        // Cast a ray downwards
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, Mathf.Infinity, groundLayer);

        RaycastHit2D hit = new RaycastHit2D();

        // Check if the ray hit a collider
        if (hits.Length > 0)
        {
            //check if the first hit is the block itself
            if (hits[0].collider.gameObject == gameObject)
            {
                if (hits.Length > 1)
                {
                    hit = hits[1];
                }
                else
                {
                    hit = new RaycastHit2D();
                }
            }
            else
            {
                hit = hits[0];
            }
        }

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
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up, Mathf.Infinity, groundLayer);

        RaycastHit2D hit = new RaycastHit2D();

        // Check if the ray hit a collider
        if (hits.Length > 0)
        {
            //check if the first hit is the block itself
            if (hits[0].collider.gameObject == gameObject)
            {

                if (hits.Length > 1)
                {
                    hit = hits[1];
                }
                else
                {
                    hit = new RaycastHit2D();
                }
            }
            else
            {
                hit = hits[0];
            }
        }

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
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, surfaceYVal + boxCollider.bounds.size.y / 2, 0);
    }

    public void SnapToCeiling(RaycastHit2D hitRay)
    {
        float ceilingYVal = getColliderCeiling(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, ceilingYVal - boxCollider.bounds.size.y/2 - 1, 0);
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
