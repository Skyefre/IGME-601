using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static Enemy;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Run,
        Jumpsquat,
        Jump,
        Landing,
        Hitstun,
        Shield,
        SideAttack,
        UpAttack,
        DownAttack,
        SpellAttack,
    }

    //Animation info fields
    public AnimatorStateInfo animStateInfo;
    public AnimatorClipInfo[] currentClipInfo;
    private int currentFrame;
    private int frameCount;
    private int hitstunVal = 0;

    //weapon and color swapping support fields
    public string enemyName = "ice";
    public Animator animator;
    //public AnimatorController baseAnimController;
    public RuntimeAnimatorController baseAnimController;
    public List<AnimatorOverrideController> otherEnemyAnimControllers;
    public List<Texture2D> colorPalletes;
    public EnemyJSONReader characterJSON;
    public EnemyJSONReader.FrameDataContainer frameData;
    public EnemyJSONReader.HitboxDataContainer hitboxData;
    public EnemyJSONReader.HurtboxDataContainer hurtboxData;
    public int maxHitboxes = 2;
    private EnemyJSONReader.EnemyDataList enemyData;
    /*private int currentAnimControllerIndex = 0;
    private int currentColorIndex = 0;*/

    //Enemy fields
    public int runSpeed = 3;
    public int jumpForce = 10;
    public int gravity = 1;
    public int health = 100;
    public int hspd = 0;
    public int vspd = 0;
    public int maxHspd = 10;
    public int maxVspd = 1;
    private Vector3 rightPosition;
    private Vector3 leftPosition;
    private Vector3 currentTarget;
    public EnemyState state = EnemyState.Idle;
    public BaseSpell currentSpell;
    public BaseSpell[] EnemySpells;
    public bool facingRight = true;
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public RaycastHit2D grounded;
    public RaycastHit2D collidedCeiling;
    public GameObject hitboxReference;
    public GameObject hurtboxReference;
    private List<GameObject> hitboxes = new List<GameObject>();
    private GameObject hurtbox;


    private int tempHspd = 0;
    public int hitstopVal = 0;
    private EnemyState prevState;
    private int lerpDelay = 0;
    //private int gravityDelay = 1;
    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component

    public BoxCollider2D EnemyCollider
    {
        get => boxCollider;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (characterJSON == null)
        {
            characterJSON = gameObject.GetComponent<EnemyJSONReader>();
        }
        characterJSON.GetEnemyStats();
        enemyData = characterJSON.enemyDataList;
        InitEnemy();
        SetState(EnemyState.Idle);

        // Set the patrol positions relative to the initial position
        leftPosition = transform.position;
        rightPosition = transform.position;
        leftPosition.x -= 80;
        rightPosition.x += 300;

        // Start by targeting the right position
        currentTarget = rightPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }

    void FixedUpdate()
    {
        if (hitstopVal > 0)
        {
            hitstopVal--;
            return;
        }
        else
        {
            animator.enabled = true;
        }

        //update animator info things to get current frame and frame count
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        frameCount = (int)(currentClipInfo[0].clip.length * currentClipInfo[0].clip.frameRate);
        currentFrame = ((int)(animStateInfo.normalizedTime * frameCount)) % frameCount;

        grounded = IsGrounded();
        if (grounded.collider == null)
        {
            vspd -= gravity;
            if (vspd < -maxVspd)
            {
                vspd = -maxVspd;
            }
        }

        switch (state)
        {

            case EnemyState.Idle:

               

                LerpHspd(0, 1);
                //check for ground
                if (grounded.collider == null)
                {
                    SetState(EnemyState.Jump);
                    break;
                }
                break;

            case EnemyState.Run:

                
                //check for collision
                if (grounded.collider == null)
                {
                    //if not grounded
                    SetState(EnemyState.Jump);
                    break;
                }
                else
                {
                    SetState(EnemyState.Idle);
                }
                break;
            case EnemyState.Jumpsquat:

                if (hspd != 0)
                {
                    tempHspd = hspd;
                    hspd = 0;
                }
                
                if (currentFrame == frameCount - 1 && grounded.collider != null)
                {
                    hspd = tempHspd;
                    tempHspd = 0;
                    vspd = jumpForce;
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(EnemyState.Jump);
                }

                break;
            case EnemyState.Jump:

                #region ground collision check
                //cast ray to check for ground and get variables for return values
                //RaycastHit2D groundHitRayHit = IsGrounded();
                Vector2? collideGroundPoint = grounded.collider != null ? (Vector2?)grounded.point : null;
                Collider2D collidedGround = grounded.collider;

                //check for ground differing based on if colider is on slope or flat ground
                if (collidedGround != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    SetState(EnemyState.Landing);
                    break;
                }
                #endregion

                
                
                
                break;

            case EnemyState.Landing:
                vspd = 0;
                hspd = 0;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(EnemyState.Idle);
                }

                break;
            case EnemyState.Hitstun:
                if (grounded.collider != null && vspd < 0)
                {
                    SnapToSurface(grounded);
                    vspd = -vspd;
                   // LerpHspd(0, 3);
                }
                if (IsTouchingWall().collider != null)
                {
                    hspd = -hspd;
                }
                if (hitstunVal > 0)
                {
                    hitstunVal--;
                }
                else
                {
                    SetState(grounded.collider != null ? EnemyState.Idle : EnemyState.Jump);
                }
                break;
            case EnemyState.Shield:
                //check for ground collision
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                }
                //check for wall collision
                if (IsTouchingWall().collider != null)
                {
                    hspd = -hspd;
                }


                LerpHspd(0, 10);
                break;
            case EnemyState.SideAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.sideAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.sideAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().hitboxActive = true;
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.sideAttackHitboxes[i].xOffset,
                            hitboxData.sideAttackHitboxes[i].yOffset,
                            hitboxData.sideAttackHitboxes[i].width,
                            hitboxData.sideAttackHitboxes[i].height,
                            hitboxData.sideAttackHitboxes[i].xKnockback,
                            hitboxData.sideAttackHitboxes[i].yKnockback,
                            hitboxData.sideAttackHitboxes[i].hitstun
                        );
                    }
                    else if (currentFrame == frameData.sideAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);

                    }

                }


                //if grounded
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == EnemyState.Jump || prevState == EnemyState.Jumpsquat)
                    {

                        SetState(EnemyState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 3);
                        
                    }

                }
                
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(grounded.collider != null ? EnemyState.Idle : EnemyState.Jump);
                    break;
                }
                break;
            case EnemyState.UpAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.upAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.upAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().hitboxActive = true;
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.upAttackHitboxes[i].xOffset,
                            hitboxData.upAttackHitboxes[i].yOffset,
                            hitboxData.upAttackHitboxes[i].width,
                            hitboxData.upAttackHitboxes[i].height,
                            hitboxData.upAttackHitboxes[i].xKnockback,
                            hitboxData.upAttackHitboxes[i].yKnockback,
                            hitboxData.upAttackHitboxes[i].hitstun
                        );
                    }
                    else if (currentFrame == frameData.upAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);
                    }

                }



                if (grounded.collider != null)
                {

                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == EnemyState.Jump || prevState == EnemyState.Jumpsquat)
                    {

                        SetState(EnemyState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 1);

                        
                    }

                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? EnemyState.Idle : EnemyState.Jump);
                    break;
                }
                break;
            case EnemyState.DownAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.downAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.downAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().hitboxActive = true;
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.downAttackHitboxes[i].xOffset,
                            hitboxData.downAttackHitboxes[i].yOffset,
                            hitboxData.downAttackHitboxes[i].width,
                            hitboxData.downAttackHitboxes[i].height,
                            hitboxData.downAttackHitboxes[i].xKnockback,
                            hitboxData.downAttackHitboxes[i].yKnockback,
                            hitboxData.downAttackHitboxes[i].hitstun
                        );
                    }
                    else if (currentFrame == frameData.downAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);
                    }

                }
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == EnemyState.Jump || prevState == EnemyState.Jumpsquat)
                    {

                        SetState(EnemyState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 2);
                    }

                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? EnemyState.Idle : EnemyState.Jump);
                    break;
                }
                break;
            case EnemyState.SpellAttack:
               
                SetState(EnemyState.Idle);
                break;
        }

        //check for ground collision
        if (grounded.collider != null)
        {
            SnapToSurface(grounded);
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
    }

    private void SetState(EnemyState targetState)
    {
        animator.enabled = true;
        animator.SetInteger("enemy_state", (int)targetState);
        prevState = state;
        state = targetState;


        //----------------------------any State specific enter and exit logic----------------------
        switch (targetState)
        {
            case EnemyState.Idle:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.idleHurtbox.xOffset,
                hurtboxData.idleHurtbox.yOffset,
                hurtboxData.idleHurtbox.width,
                hurtboxData.idleHurtbox.height
                );
                break;
            case EnemyState.Run:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.runHurtbox.xOffset,
                hurtboxData.runHurtbox.yOffset,
                hurtboxData.runHurtbox.width,
                hurtboxData.runHurtbox.height
                );
                break;
            case EnemyState.Jumpsquat:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpsquatHurtbox.xOffset,
                hurtboxData.jumpsquatHurtbox.yOffset,
                hurtboxData.jumpsquatHurtbox.width,
                hurtboxData.jumpsquatHurtbox.height
                );
                break;
            case EnemyState.Jump:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpHurtbox.xOffset,
                hurtboxData.jumpHurtbox.yOffset,
                hurtboxData.jumpHurtbox.width,
                hurtboxData.jumpHurtbox.height
                );
                break;
            case EnemyState.Landing:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.landingHurtbox.xOffset,
                hurtboxData.landingHurtbox.yOffset,
                hurtboxData.landingHurtbox.width,
                hurtboxData.landingHurtbox.height
                );
                break;
            case EnemyState.Hitstun:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.hitstunHurtbox.xOffset,
                hurtboxData.hitstunHurtbox.yOffset,
                hurtboxData.hitstunHurtbox.width,
                hurtboxData.hitstunHurtbox.height
                );
                // hitstunVal = 60;
                break;
            case EnemyState.Shield:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.shieldHurtbox.xOffset,
                hurtboxData.shieldHurtbox.yOffset,
                hurtboxData.shieldHurtbox.width,
                hurtboxData.shieldHurtbox.height
                );
                break;
            case EnemyState.SideAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.sideAttackHurtbox.xOffset,
                hurtboxData.sideAttackHurtbox.yOffset,
                hurtboxData.sideAttackHurtbox.width,
                hurtboxData.sideAttackHurtbox.height
                );
                break;
            case EnemyState.UpAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.upAttackHurtbox.xOffset,
                hurtboxData.upAttackHurtbox.yOffset,
                hurtboxData.upAttackHurtbox.width,
                hurtboxData.upAttackHurtbox.height
                );
                break;
            case EnemyState.DownAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.downAttackHurtbox.xOffset,
                hurtboxData.downAttackHurtbox.yOffset,
                hurtboxData.downAttackHurtbox.width,
                hurtboxData.downAttackHurtbox.height
                );
                break;
        }

        switch (prevState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Run:
                break;
            case EnemyState.Jumpsquat:
                break;
            case EnemyState.Jump:
                break;
            case EnemyState.Landing:
                break;
            case EnemyState.Hitstun:
                break;
            case EnemyState.Shield:
                break;
            case EnemyState.SideAttack:
                DisableAllHitboxes();
                break;
            case EnemyState.UpAttack:
                DisableAllHitboxes();
                break;
            case EnemyState.DownAttack:
                DisableAllHitboxes();
                break;
        }
    }

    #region Collision Detection

    private RaycastHit2D IsGrounded()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = -(vspd - 5);

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.min.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.min.y);

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
        Vector2 topLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.max.y - rayOffset.y);
        Vector2 bottomLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.min.y + rayOffset.y);
        Vector2 centerLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.center.y);
        Vector2 topRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.max.y - rayOffset.y);
        Vector2 bottomRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.min.y + rayOffset.y);
        Vector2 centerRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.center.y);


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
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.max.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.max.y);

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
        Vector2 rayOrigin = new Vector2(xValue, targetCollider.bounds.max.y + 1); // Adjust the y value as needed

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

    public float GetColliderSurface(float xValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(xValue, targetCollider.bounds.max.y + 1); // Adjust the y value as needed

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
        Vector2 rayOrigin = new Vector2(xValue, targetCollider.bounds.min.y - 1); // Adjust the y value as needed

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
        float surfaceYVal = this.GetColliderSurface(hitRay.point.x, hitRay.collider);
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

    void InitEnemy()
    {

        for (int i = 0; i < enemyData.enemyData.Count; i++)
        {
            if (enemyData.enemyData[i].enemy == enemyName)
            {
                runSpeed = enemyData.enemyData[i].runSpeed;
                jumpForce = enemyData.enemyData[i].jumpForce;
                maxHitboxes = enemyData.enemyData[i].maxHitboxes;
                frameData = enemyData.enemyData[i].frameData;
                hitboxData = enemyData.enemyData[i].hitboxData;
                hurtboxData = enemyData.enemyData[i].hurtboxData;
            }
        }
        InitHitboxes();
        InitHurtbox();
    }

    void InitHitboxes()
    {
        if (hitboxes.Count >= maxHitboxes)
        {
            return;
        }
        for (int i = 0; i < maxHitboxes; i++)
        {
            GameObject hitbox = Instantiate(hitboxReference, gameObject.transform);
            hitbox.GetComponent<Hitbox>().owner = gameObject;
            hitbox.GetComponent<Hitbox>().hitboxActive = false;
            hitbox.GetComponent<Hitbox>().damage = 0;
            hitbox.GetComponent<Hitbox>().xoffset = 0;
            hitbox.GetComponent<Hitbox>().yoffset = 0;
            hitbox.GetComponent<Hitbox>().width = 0;
            hitbox.GetComponent<Hitbox>().height = 0;
            hitbox.GetComponent<Hitbox>().xKnockback = 0;
            hitbox.GetComponent<Hitbox>().yKnockback = 0;
            hitbox.GetComponent<Hitbox>().hitstun = 0;
            hitbox.SetActive(false);
            hitboxes.Add(hitbox);
        }

    }
    void InitHurtbox()
    {
        if (hurtbox != null)
        {
            return;
        }
        hurtbox = Instantiate(hurtboxReference, gameObject.transform);
        hurtbox.GetComponent<Hurtbox>().owner = gameObject;
        hurtbox.GetComponent<Hurtbox>().hurtboxActive = true;
        hurtbox.GetComponent<Hurtbox>().xoffset = 0;
        hurtbox.GetComponent<Hurtbox>().yoffset = 0;
        hurtbox.GetComponent<Hurtbox>().width = 0;
        hurtbox.GetComponent<Hurtbox>().height = 0;
        hurtbox.SetActive(true);

    }

    void DisableAllHitboxes()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
        }

    }

    public void TakeDamage(GameObject hitEnemy, int damage, int xKnockback, int yKnockback, int hitstun)
    {

        //If this enemy is block and facing the right direction
        if (state == EnemyState.Shield &&
            ((hitEnemy.transform.position.x > gameObject.transform.position.x && facingRight) ||
            (hitEnemy.transform.position.x < gameObject.transform.position.x && !facingRight)))
        {
            hspd = xKnockback / 2;
        }
        else
        {
            health -= damage;
            hspd = xKnockback;
            vspd = yKnockback;
            hitstunVal = hitstun;
            SetState(EnemyState.Hitstun);
        }

        Debug.Log("Enemy Health: " + health);
    }

    public virtual void Patrolling()
    {
        // Determine the horizontal speed based on the current target position
        if (currentTarget == rightPosition)
        {
            hspd = runSpeed/2;
        }
        else
        {
            hspd = -runSpeed/2;
        }

        // Switch target when close to the current one
        if (Vector2.Distance(transform.position, currentTarget) < 5f)
        {
            if(currentTarget == rightPosition)
            {
                currentTarget = leftPosition;
                facingRight = false;
            }
            else
            {
                currentTarget = rightPosition;
                facingRight = true;
            }
        }
    }
}
