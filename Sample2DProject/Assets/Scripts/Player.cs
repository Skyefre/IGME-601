using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        Jumpsquat,
        Jump,
        Landing,
        Hitstun,
    }

    public enum PlayerPower
    {
        Ice,
        Wind,
    }

    //Animation fields
    public AnimatorStateInfo animStateInfo;
    public AnimatorClipInfo[] currentClipInfo;
    int currentFrame;
    int frameCount;

    public int runSpeed = 3;
    public int jumpForce = 10;
    public int gravity = 1;
    public int health = 100;
    public int hspd = 0;
    public int vspd = 0;
    public int maxHspd = 10;
    public int maxVspd = 1;
    private PlayerState prevState;
    public PlayerState state = PlayerState.Idle;
    public Animator animator;
    //public Rigidbody2D rb;
    public bool facingRight = true;
    public InputHandler inputHandler;
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public RaycastHit2D grounded;

    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component
    private Dictionary<InputHandler.Inputs, InputHandler.InputState> inputs;

    private void Awake()
    {
        inputs = inputHandler.keyBindings;
    }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    void FixedUpdate()
    {
        //update this frames inputs
        inputs = inputHandler.keyBindings;

        //update animator info things to get current frame and frame count
        //dt = Time.deltaTime;
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        frameCount = (int)(currentClipInfo[0].clip.length * currentClipInfo[0].clip.frameRate);
        currentFrame = (Mathf.RoundToInt(animStateInfo.normalizedTime * frameCount)) % frameCount;
        //Debug.Log(currentFrame);


        switch (state)
        {
            
            case PlayerState.Idle:

                //check for input
                if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    //hspd = -runSpeed;
                    SetState(PlayerState.Run);
                    facingRight = false;
                }
                else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    //hspd = runSpeed;
                    SetState(PlayerState.Run);
                    facingRight = true;
                }
                else if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Jumpsquat);
                }

                break;
            case PlayerState.Run:
                //check for collision
                grounded = IsGrounded();
                if (grounded.collider == null)
                {
                    //if not grounded
                    SetState(PlayerState.Jump);
                    break;
                }
                //run logic
                if (facingRight)
                {
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = runSpeed;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        hspd = 0;
                        SetState(PlayerState.Idle);
                        break;
                    }
                }
                else
                {
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = -runSpeed;
                    }
                    else if (inputHandler.keyBindings[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed)
                    {
                        hspd = 0;
                        SetState(PlayerState.Idle);
                    }
                }
                if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Jumpsquat);
                }
                break;
            case PlayerState.Jumpsquat:
                int tempHspd = hspd;
                hspd = 0;
                if (currentFrame == frameCount - 1 && grounded.collider != null)
                {
                    hspd = tempHspd;
                    vspd = jumpForce;
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(PlayerState.Jump);
                }

                break;
            case PlayerState.Jump:
                vspd -= gravity;
                if (vspd < -maxVspd)
                {
                    vspd = -maxVspd;
                }

                //check for ceiling
                Collider2D collidedCeiling = IsTouchingCeiling().collider;
                if (collidedCeiling != null)
                {
                    vspd = 0;
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, collidedCeiling.bounds.min.y - (boxCollider.size.y * gameObject.transform.localScale.y) - 1, 0);
                }
                #region ground collision check
                //cast ray to check for ground and get variables for return values
                RaycastHit2D groundHitRayHit = IsGrounded();
                Vector2? collideGroundPoint = groundHitRayHit.collider != null ? (Vector2?)groundHitRayHit.point : null;
                Collider2D collidedGround = groundHitRayHit.collider;

                //check for ground differing based on if colider is on slope or flat ground
                if (collidedGround != null)
                {
                    //if (collidedGround.gameObject.tag == "slope")
                    //{
                    //    //if slope
                    //    vspd = 0;
                    //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, collideGroundPoint.Value.y, 0);
                    //}
                    //else
                    //{
                    //    //if flat ground
                    //    vspd = 0;
                    //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, collidedGround.bounds.max.y, 0);
                    //}
                    SnapToSurface(groundHitRayHit);
                    vspd = 0;
                    SetState(PlayerState.Landing);
                    break;
                }
                #endregion

                //allow for horizontal movement
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                    hspd = runSpeed;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                    hspd = -runSpeed;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                {
                    hspd = 0;
                }

                break;
            case PlayerState.Landing:
                vspd = 0;
                hspd = 0;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.Run);
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.Run);
                        facingRight = true;
                    }
                    else
                    {
                        SetState(PlayerState.Idle);
                    }

                }

                break;
        }

        grounded = IsGrounded();
        if (grounded.collider == null)
        {
            //if not grounded
            SetState(PlayerState.Jump);
        }
        else
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

        gameObject.transform.position += new Vector3(hspd, vspd, 0);
        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
    }

    private void SetState(PlayerState targetState)
    {
        animator.enabled = true;
        animator.SetInteger("player_state", (int)targetState);
        prevState = state;
        state = targetState;
        //----------------------------any State specific enter and exit logic----------------------
        //if (prevState == PlayerState.Jumpsquat && state == PlayerState.Jump)
        //{
        //    //apply jumpforce
        //    vspd = jumpForce;
        //}

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
            if(leftHit.point.y > rightHit.point.y)
            {
                return leftHit;
            }
            else if (leftHit.point.y < rightHit.point.y)
            {
                return rightHit;
            }
            else
            {
                return facingRight? rightHit:leftHit;
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
        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.left, rayLength, groundLayer);
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.left, rayLength, groundLayer);
        RaycastHit2D centerLeftHit = Physics2D.Raycast(centerLeftRayOrigin, Vector2.left, rayLength, groundLayer);
        RaycastHit2D topRightHit = Physics2D.Raycast(topRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D bottomRightHit = Physics2D.Raycast(bottomRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D centerRightHit = Physics2D.Raycast(centerRightRayOrigin, Vector2.right, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(topLeftRayOrigin, Vector2.left * rayLength, Color.blue);
        Debug.DrawRay(bottomLeftRayOrigin, Vector2.left * rayLength, Color.blue);
        Debug.DrawRay(centerLeftRayOrigin, Vector2.left * rayLength, Color.blue);
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
    #endregion

    public void SnapToSurface(RaycastHit2D hitRay)
    {
        float surfaceYVal = getColliderSurface(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, surfaceYVal, 0);
    }
}
