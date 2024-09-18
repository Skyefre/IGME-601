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
    }
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
    public Vector2 rayOffset = new Vector2(0.1f, 0f); // Offset for the rays

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

        switch (state)
        {
            case PlayerState.Idle:
                //check for collision
                if (IsGrounded() == null)
                {
                    //if not grounded
                    SetState(PlayerState.Jump);
                    break;
                }

                //check for input
                if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Pressed)
                {
                    //hspd = -runSpeed;
                    SetState(PlayerState.Run);
                    if (facingRight)
                    {
                        facingRight = false;
                    }
                }
                else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Pressed)
                {
                    //hspd = runSpeed;
                    SetState(PlayerState.Run);
                    if (!facingRight)
                    {
                        facingRight = true;
                    }
                }
                else if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                {
                    SetState(PlayerState.Jumpsquat);
                }

                break;
            case PlayerState.Run:
                //check for ground under player
                if (IsGrounded() == null)
                {
                    //if not grounded
                    SetState(PlayerState.Jump);
                    break;
                }
                //run logic
                if (facingRight)
                {
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Pressed)
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
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Pressed)
                    {
                        hspd = -runSpeed;
                    }
                    else if (inputHandler.keyBindings[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed)
                    {
                        hspd = 0;
                        SetState(PlayerState.Idle);
                    }
                }
                if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                {
                    SetState(PlayerState.Jumpsquat);
                }
                break;
            case PlayerState.Jumpsquat:
                SetState(PlayerState.Jump);
                break;
            case PlayerState.Jump:
                vspd -= gravity;
                if (vspd < -maxVspd)
                {
                    vspd = -maxVspd;
                }

                //check for ceiling
                Collider2D collidedCeiling = IsTouchingCeiling();
                if (collidedCeiling != null)
                {
                    vspd = 0;
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, collidedCeiling.bounds.min.y - (boxCollider.size.y * gameObject.transform.localScale.y / 2) - 1, 0);
                }
                Collider2D collidedGround = IsGrounded();
                if (collidedGround != null)
                {
                    vspd = 0;
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, collidedGround.bounds.max.y + (boxCollider.size.y * gameObject.transform.localScale.y / 2), 0);
                    SetState(PlayerState.Landing);

                }
                //allow for horizontal movement

                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Pressed)
                {
                    facingRight = true;
                    hspd = runSpeed;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Pressed)
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
                SetState(PlayerState.Idle);
                break;
        }

        //check horizontal collision
        Collider2D collidedWall = IsTouchingWall();
        if (collidedWall != null)
        {
            hspd = 0;
        }

        gameObject.transform.position += new Vector3(hspd, vspd, 0);
    }

    private void SetState(PlayerState targetState)
    {
        prevState = state;
        state = targetState;
        //----------------------------any State specific enter and exit logic----------------------
        if (prevState == PlayerState.Jumpsquat && state == PlayerState.Jump)
        {
            //apply jumpforce
            vspd = jumpForce;
        }

    }

    #region Collision Detection
    private Collider2D IsGrounded()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = -(vspd - 1);

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.min.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.min.y);

        // Cast rays downwards
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);



        // Return true if either ray hits the ground
        if (leftHit.collider != null)
        {
            return leftHit.collider;
        }
        else if (rightHit.collider != null)
        {
            return rightHit.collider;
        }
        return null;
    }


    private Collider2D IsTouchingWall()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = hspd;

        // Calculate the positions for the rays on the left and right sides
        Vector2 leftRayOrigin = new Vector2(bounds.min.x, bounds.center.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x, bounds.center.y);

        // Cast rays to the left and right
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.right, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.left * rayLength, Color.blue);
        Debug.DrawRay(rightRayOrigin, Vector2.right * rayLength, Color.blue);

        // Return the collider if any of the rays hit a wall
        if (leftHit.collider != null && hspd < 0)
        {
            return leftHit.collider;
        }
        else if (rightHit.collider != null && hspd > 0)
        {
            return rightHit.collider;
        }
        else
        {
            return null;
        }
    }

    private Collider2D IsTouchingCeiling()
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
            return leftHit.collider;
        }
        else if (rightHit.collider != null)
        {
            return rightHit.collider;
        }
        else
        {
            return null;
        }
    }
    #endregion
}
