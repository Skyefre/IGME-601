using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static Enemy;

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
        Shield,
        SideAttack,
        UpAttack,
        DownAttack,
        Menuing,
        SpellAttack,
        SpellUtil
    }

    

    //Animation info fields
    public AnimatorStateInfo animStateInfo;
    public AnimatorClipInfo[] currentClipInfo;
    private int currentFrame;
    private int frameCount;
    private int hitstunVal = 0;

    //weapon and color swapping support fields
    public string weaponName = "ice";
    public Animator animator;
    //public AnimatorController baseAnimController;
    public RuntimeAnimatorController baseAnimController;
    public List<AnimatorOverrideController> otherWeaponAnimControllers;
    public List<Texture2D> colorPalletes;
    public PlayerJSONReader characterJSON;
    public PlayerJSONReader.FrameDataContainer frameData;
    public PlayerJSONReader.HitboxDataContainer hitboxData;
    public PlayerJSONReader.HurtboxDataContainer hurtboxData;
    public PlayerJSONReader.ImpulseFrames impulseFrames;
    public PlayerJSONReader.ImpulseDataContainer impulseData;
    public PlayerJSONReader.SpellFrameData spellframeData;
    public PlayerJSONReader.SpellSpawnDataContainer spellSpawnData;
    public int maxHitboxes = 1;
    private PlayerJSONReader.WeaponDataList weaponData;
    private int currentAnimControllerIndex = 0;
    private int currentColorIndex = 0;
    private AudioSource jump;

    //Player fields
    public int runSpeed = 3;
    public float jumpForce = 10;
    public float gravity = 1;
    public int health = 10;
    public int hspd = 0;
    public float vspd = 0;
    public int maxHspd = 10;
    public int maxVspd = 1;
    public PlayerState state = PlayerState.Idle;
    
    //public BaseSpell currentSpell;
    //public BaseSpell[] PlayerSpells;
    public bool facingRight = true;
    public bool InTeleport = false;
    public InputHandler inputHandler;
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public RaycastHit2D grounded;
    public RaycastHit2D collidedCeiling;
    public GameObject hitboxReference;
    public GameObject hurtboxReference;
    private List<GameObject> hitboxes = new List<GameObject>();
    private GameObject hurtbox;
    public GameObject iceBlock;
    public bool isAlive = true;

    private int tempHspd = 0;
    public int hitstopVal = 0;
    private PlayerState prevState;
    private int lerpDelay = 0;
    //private bool preview = false;
    //private int gravityDelay = 1;
    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component
    private Dictionary<InputHandler.Inputs, InputHandler.InputState> inputs;

    //Spell stuff
    public List<GameObject> projectileList = new List<GameObject>();
    public Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();
    public int spellCharge = 0;
    public int maxSpellCharge = 100;

    //VFX stuff
    public List<GameObject> vfxList = new List<GameObject>();
    public Dictionary<string, GameObject> vfxEntities = new Dictionary<string, GameObject>();

    public BoxCollider2D PlayerCollider
    {
        get => boxCollider;
    }
    private void Awake()
    {
        inputs = inputHandler.keyBindings;
        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", colorPalletes[GameManager.Instance.players.Length == 1 ? 0 : 1]);

    }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (characterJSON == null)
        {
            characterJSON = gameObject.GetComponent<PlayerJSONReader>();
        }
        if(jump == null)
        {
            jump = gameObject.GetComponent<AudioSource>();
        }
        characterJSON.GetWeaponStats();
        weaponData = characterJSON.weaponDataList;
        InitWeapon();
        SetState(PlayerState.Idle);
    }

    void FixedUpdate()
    {
        //update this frames inputs
        inputs = inputHandler.keyBindings;
        if (hitstopVal > 0)
        {
            hitstopVal--;
            return;
        }
        else if (!isAlive)
        {
            //check for menu input
            if (inputs[InputHandler.Inputs.Pause] == InputHandler.InputState.Pressed && GameManager.Instance.stockCount >0)
            {
                Respawn();
            }
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

        
        //re-update weapon stats when pause is pressed
        if (Input.GetKey(KeyCode.F1))
        {
            characterJSON.GetWeaponStats();
            weaponData = characterJSON.weaponDataList;
            InitWeapon();
        }

        //get spell button for the charge system
        if (inputs[InputHandler.Inputs.Spell] == InputHandler.InputState.Held)
        {
            //spellCharge++;
            //if(spellCharge >= maxSpellCharge)
            //{
            //    spellCharge = maxSpellCharge + 4;
            //    //make the character flash white
            //}
            if (!vfxEntities["spell_charge"].activeSelf)
            {
                vfxEntities["spell_charge"].SetActive(true);
                vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().InitProjectile((int)transform.position.x, (int)(transform.position.y + boxCollider.size.y / 2));
            }
            
        }
        else
        {
            //if(spellCharge > 0)
            //{
            //    spellCharge-=2;
            //}
            //if (vfxEntities["spell_charge"].activeSelf)
            //{
            //    vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().DestroyProjectile();
            //}

        }

        switch (state)
        {

            case PlayerState.Idle:

                /*if (inputs[InputHandler.Inputs.Select] == InputHandler.InputState.Pressed)
                {
                    Debug.Log("Selected");
                }*/

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }

                }
                //check for spell input
                if (inputs[InputHandler.Inputs.Spell] == InputHandler.InputState.Released)
                {
                    Debug.Log(animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
                    if(vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().animator.GetCurrentAnimatorStateInfo(0).IsName("spell_charged"))
                    {
                        SetState(PlayerState.SpellUtil);
                    }
                    else
                    {
                        SetState(PlayerState.SpellAttack);
                    }

                    break;
                }
                //check for movement input
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


                //check for shield input
                if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Shield);
                }

                //check for menu input
                if (inputs[InputHandler.Inputs.Pause] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Menuing);
                }

                LerpHspd(0, 1);
                //check for ground
                if (grounded.collider == null)
                {
                    SetState(PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.Run:

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for spell input
                if (inputs[InputHandler.Inputs.Spell] == InputHandler.InputState.Released)
                {
                    if (/*spellCharge >= maxSpellCharge*/vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().animator.GetCurrentAnimatorStateInfo(0).IsName("spell_charged"))
                    {
                        SetState(PlayerState.SpellUtil);
                    }
                    else
                    {
                        SetState(PlayerState.SpellAttack);
                    }

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
                //check for jump input
                if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Jumpsquat);
                }
                //check for shield input
                if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                {
                    SetState(PlayerState.Shield);
                }
                //check for collision
                if (grounded.collider == null)
                {
                    //if not grounded
                    SetState(PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.Jumpsquat:


                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    if (tempHspd != 0)
                    {
                        hspd = tempHspd;
                        tempHspd = 0;
                    }
                    vspd = jumpForce;
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for shield input
                if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                {
                    if (tempHspd != 0)
                    {
                        hspd = tempHspd;
                        tempHspd = 0;
                    }
                    vspd = jumpForce;
                    SetState(PlayerState.Shield);
                }
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                }
                if (currentFrame == frameCount - 1 && grounded.collider != null)
                {
                    if (tempHspd != 0)
                    {
                        hspd = tempHspd;
                        tempHspd = 0;
                    }

                    vspd = (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held ? jumpForce : jumpForce / 2);
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(PlayerState.Jump);
                }

                break;
            case PlayerState.Jump:

                //check for ground collision
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    SetState(PlayerState.Landing);
                    break;
                }

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    //check for turnaround inputs
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        facingRight = true;
                    }

                    //check for which attack is pressed
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for spell input
                if (inputs[InputHandler.Inputs.Spell] == InputHandler.InputState.Released)
                {
                    if (/*spellCharge >= maxSpellCharge*/vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().animator.GetCurrentAnimatorStateInfo(0).IsName("spell_charged"))
                    {
                        SetState(PlayerState.SpellUtil);
                    }
                    else
                    {
                        SetState(PlayerState.SpellAttack);
                    }

                    break;
                }
                //allow for horizontal movement
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                    hspd = hspd < runSpeed ? runSpeed : hspd;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                    hspd = hspd > -runSpeed ? -runSpeed : hspd;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                {
                    LerpHspd(0, 5);
                }
                //check for shield input
                if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Held)
                {
                    SetState(PlayerState.Shield);
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
            case PlayerState.Hitstun:
                if (grounded.collider != null && vspd < 0)
                {
                    SnapToSurface(grounded);
                    vspd = -vspd;
                    //LerpHspd(0, 3);
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
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                }
                break;
            case PlayerState.Shield:
                //check for ground collision
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    //check for Jump input
                    if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                    {
                        SetState(PlayerState.Jumpsquat);
                    }
                }
                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for wall collision
                if (IsTouchingWall().collider != null)
                {
                    hspd = -hspd;
                }
                //check for shield release
                if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.UnPressed)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                }

                //check for horizontal inputs
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                }


                LerpHspd(0, 10);
                break;
            case PlayerState.SideAttack:

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
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 3);
                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }

                //handle impulse activation
                for (int i = 0; i < impulseFrames.sideAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.sideAttackImpulseFrames[i])
                    {

                        vspd = impulseData.sideAttackImpulseData[i].yImpulse;
                        hspd = impulseData.sideAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                
                break;
            case PlayerState.UpAttack:

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
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 1);

                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }

                //handle impulse activation
                for (int i = 0; i < impulseFrames.upAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.upAttackImpulseFrames[i])
                    {

                        vspd = impulseData.upAttackImpulseData[i].yImpulse;
                        hspd = impulseData.upAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.DownAttack:

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
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 2);
                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = runSpeed;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = -runSpeed;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        hspd = 0;
                    }
                }
                //handle impulse activation
                for (int i = 0; i < impulseFrames.downAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.downAttackImpulseFrames[i])
                    {

                        vspd = impulseData.downAttackImpulseData[i].yImpulse;
                        hspd = impulseData.downAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.SpellAttack:
                ///handle hitbox activation
                for (int i = 0; i < spellframeData.spellAttackFrames.Count; i++)
                {
                    if (currentFrame == spellframeData.spellAttackFrames[i])
                    {
                        switch (weaponName)
                        {
                            case "ice":
                                if (!projectiles["ice_attack"].activeSelf && !projectiles["ice_attack"].GetComponent<IceAttackProjectile>().projectileActive)
                                {
                                    projectiles["ice_attack"].SetActive(true);
                                    projectiles["ice_attack"].GetComponent<IceAttackProjectile>().InitProjectile(spellSpawnData.spellAttack[0].xOffset, spellSpawnData.spellAttack[0].yOffset);
                                }
                                break;
                            case "wind":
                                if (!projectiles["wind_attack"].activeSelf && !projectiles["wind_attack"].GetComponent<WindAttackProjectile>().projectileActive)
                                {
                                    projectiles["wind_attack"].SetActive(true);
                                    projectiles["wind_attack"].GetComponent<WindAttackProjectile>().InitProjectile(spellSpawnData.spellAttack[0].xOffset, spellSpawnData.spellAttack[0].yOffset);
                                }
                                break;
                            case "lightning":
                                break;
                            case "fire":
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (weaponName)
                        {
                            case "ice":
                                projectiles["ice_attack"].GetComponent<IceAttackProjectile>().projectileActive = false;
                                break;
                            case "wind":
                                projectiles["wind_attack"].GetComponent<WindAttackProjectile>().projectileActive = false;
                                break;
                            case "lightning":
                                //projectiles["lightning_attack"].GetComponent<LightningAttackProjectile>().projectileActive = false;
                                break;
                            case "fire":
                                //projectiles["fire_attack"].GetComponent<FireAttackProjectile>().projectileActive = false;
                                break;
                            default:
                                break;
                        }
                    }

                }


                //if grounded
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    LerpHspd(0, 3);

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }
                //handle impulse activation
                for (int i = 0; i < impulseFrames.spellAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.spellAttackImpulseFrames[i])
                    {

                        vspd = impulseData.spellAttackImpulseData[i].yImpulse;
                        hspd = impulseData.spellAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.SpellUtil:
                ///handle hitbox activation
                for (int i = 0; i < spellframeData.spellUtilFrames.Count; i++)
                {
                    if (currentFrame == spellframeData.spellUtilFrames[i])
                    {
                        switch (weaponName)
                        {
                            case "ice":
                                if (projectiles["ice_util"].activeSelf == false)
                                {
                                    projectiles["ice_util"].SetActive(true);
                                    projectiles["ice_util"].GetComponent<IceBlock>().InitProjectile(spellSpawnData.spellUtil[0].xOffset, spellSpawnData.spellUtil[0].yOffset);
                                }
                                break;
                            case "wind":
                                if (projectiles["wind_util"].activeSelf == false)
                                {
                                    projectiles["wind_util"].SetActive(true);
                                    projectiles["wind_util"].GetComponent<WindUtility>().InitProjectile(spellSpawnData.spellUtil[0].xOffset, spellSpawnData.spellUtil[0].yOffset);
                                }
                                break;
                            case "lightning":
                                break;
                            case "fire":
                                break;
                            default:
                                break;
                        }
                    }

                }


                //if grounded
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    LerpHspd(0, 3);

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }

                //handle impulse activation
                for (int i = 0; i < impulseFrames.spellUtilImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.spellUtilImpulseFrames[i])
                    {

                        vspd = impulseData.spellUtilImpulseData[i].yImpulse;
                        hspd = impulseData.spellUtilImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.Menuing:
                hspd = 0;
                vspd = 0;
                if (inputs[InputHandler.Inputs.Pause] == InputHandler.InputState.Pressed)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                }

                //change weapon when attack is pressed
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
                {
                    CycleWeapon();
                }
                //change color when jump is pressed
                //if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
                //{
                //    CycleColor();
                //}

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
        //if(facingRight)
        //{
        //    iceBlock.transform.localRotation = Quaternion.Euler(0, 0, 0);        }
        //else
        //{
        //    iceBlock.transform.localRotation = Quaternion.Euler(0, 180, 0);
        //}

        //get rid of the charge animation after you release the spell button
        if (inputs[InputHandler.Inputs.Spell] == InputHandler.InputState.UnPressed)
        {
            
            if (vfxEntities["spell_charge"].activeSelf)
            {
                vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().DestroyProjectile();
            }

        }

    }

    private void SetState(PlayerState targetState)
    {
        animator.enabled = true;
        animator.SetInteger("player_state", (int)targetState);
        prevState = state;
        state = targetState;


        //----------------------------any State specific enter and exit logic----------------------
        switch (targetState)
        {
            case PlayerState.Idle:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.idleHurtbox.xOffset,
                hurtboxData.idleHurtbox.yOffset,
                hurtboxData.idleHurtbox.width,
                hurtboxData.idleHurtbox.height
                );
                break;
            case PlayerState.Run:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.runHurtbox.xOffset,
                hurtboxData.runHurtbox.yOffset,
                hurtboxData.runHurtbox.width,
                hurtboxData.runHurtbox.height
                );
                break;
            case PlayerState.Jumpsquat:
                if (hspd != 0)
                {
                    tempHspd = hspd;
                    hspd = 0;
                }
                jump.Play();
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpsquatHurtbox.xOffset,
                hurtboxData.jumpsquatHurtbox.yOffset,
                hurtboxData.jumpsquatHurtbox.width,
                hurtboxData.jumpsquatHurtbox.height
                );
                break;
            case PlayerState.Jump:
                
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpHurtbox.xOffset,
                hurtboxData.jumpHurtbox.yOffset,
                hurtboxData.jumpHurtbox.width,
                hurtboxData.jumpHurtbox.height
                );
                break;
            case PlayerState.Landing:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.landingHurtbox.xOffset,
                hurtboxData.landingHurtbox.yOffset,
                hurtboxData.landingHurtbox.width,
                hurtboxData.landingHurtbox.height
                );
                break;
            case PlayerState.Hitstun:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.hitstunHurtbox.xOffset,
                hurtboxData.hitstunHurtbox.yOffset,
                hurtboxData.hitstunHurtbox.width,
                hurtboxData.hitstunHurtbox.height
                );
                // hitstunVal = 60;
                break;
            case PlayerState.Shield:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.shieldHurtbox.xOffset,
                hurtboxData.shieldHurtbox.yOffset,
                hurtboxData.shieldHurtbox.width,
                hurtboxData.shieldHurtbox.height
                );
                break;
            case PlayerState.SideAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.sideAttackHurtbox.xOffset,
                hurtboxData.sideAttackHurtbox.yOffset,
                hurtboxData.sideAttackHurtbox.width,
                hurtboxData.sideAttackHurtbox.height
                );
                break;
            case PlayerState.UpAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.upAttackHurtbox.xOffset,
                hurtboxData.upAttackHurtbox.yOffset,
                hurtboxData.upAttackHurtbox.width,
                hurtboxData.upAttackHurtbox.height
                );
                break;
            case PlayerState.DownAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.downAttackHurtbox.xOffset,
                hurtboxData.downAttackHurtbox.yOffset,
                hurtboxData.downAttackHurtbox.width,
                hurtboxData.downAttackHurtbox.height
                );
                break;
            case PlayerState.Menuing:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.menuingHurtbox.xOffset,
                hurtboxData.menuingHurtbox.yOffset,
                hurtboxData.menuingHurtbox.width,
                hurtboxData.menuingHurtbox.height
                );
                break;
            case PlayerState.SpellAttack:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                    hurtboxData.spellAttackHurtbox.xOffset,
                    hurtboxData.spellAttackHurtbox.yOffset,
                    hurtboxData.spellAttackHurtbox.width,
                    hurtboxData.spellAttackHurtbox.height
                    );
                break;
            case PlayerState.SpellUtil:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                    hurtboxData.spellUtilHurtbox.xOffset,
                    hurtboxData.spellUtilHurtbox.yOffset,
                    hurtboxData.spellUtilHurtbox.width,
                    hurtboxData.spellUtilHurtbox.height
                    );
                break;
        }

        switch (prevState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jumpsquat:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Landing:
                break;
            case PlayerState.Hitstun:
                break;
            case PlayerState.Shield:
                break;
            case PlayerState.SideAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.UpAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.DownAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.Menuing:
                break;
            case PlayerState.SpellAttack:
                break;
            case PlayerState.SpellUtil:
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
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.max.y - 1); // Adjust the y value as needed

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

    void InitWeapon()
    {

        for (int i = 0; i < weaponData.weaponData.Count; i++)
        {
            if (weaponData.weaponData[i].weapon == weaponName)
            {
                runSpeed = weaponData.weaponData[i].runSpeed;
                jumpForce = weaponData.weaponData[i].jumpForce;
                maxHitboxes = weaponData.weaponData[i].maxHitboxes;
                frameData = weaponData.weaponData[i].frameData;
                hitboxData = weaponData.weaponData[i].hitboxData;
                hurtboxData = weaponData.weaponData[i].hurtboxData;
                impulseFrames = weaponData.weaponData[i].impulseFrames;
                impulseData = weaponData.weaponData[i].impulseData;
                spellframeData = weaponData.weaponData[i].spellframeData;
                spellSpawnData = weaponData.weaponData[i].spellSpawnData;
            }
        }
        InitHitboxes();
        InitHurtbox();
        InitEntities();
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
            hitbox.GetComponent<Hitbox>().isProjectile = false;
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

    void InitEntities()
    {
        //set all projectiles in projectile list to inactive
        //for (int i = 0; i < projectileList.Count; i++)
        //{
        //    //projectileList[i].SetActive(false);
        //    projectiles.Add(weaponName + "_attack", Instantiate(projectileList[i]));
        //    //projectileList[i].GetComponent<ProjectileBehavior>().owner = gameObject;
        //}
        
        //initialize projectile dictionary
        if(projectiles.Count < 1)
        {
            //TODO: Replace the projectile list indicies with the proper projectiles once they are added
            projectiles.Add("ice_attack", Instantiate(projectileList[0]));
            projectiles.Add("wind_attack", Instantiate(projectileList[2]));
            projectiles.Add("lightning_attack", Instantiate(projectileList[0]));
            projectiles.Add("fire_attack", Instantiate(projectileList[0]));

            projectiles.Add("ice_util", Instantiate(projectileList[1]));
            projectiles.Add("wind_util", Instantiate(projectileList[3]));
            projectiles.Add("lightning_util", Instantiate(projectileList[1]));
            projectiles.Add("fire_util", Instantiate(projectileList[1]));



            projectiles["ice_attack"].GetComponent<IceAttackProjectile>().owner = gameObject;
            projectiles["ice_attack"].SetActive(false);

            projectiles["wind_attack"].GetComponent<WindAttackProjectile>().owner = gameObject;
            projectiles["wind_attack"].SetActive(false);

            projectiles["lightning_attack"].GetComponent<IceAttackProjectile>().owner = gameObject;
            projectiles["lightning_attack"].SetActive(false);

            projectiles["fire_attack"].GetComponent<IceAttackProjectile>().owner = gameObject;
            projectiles["fire_attack"].SetActive(false);

            projectiles["ice_util"].GetComponent<IceBlock>().owner = gameObject;
            projectiles["ice_util"].SetActive(false);

            projectiles["wind_util"].GetComponent<WindUtility>().owner = gameObject;
            projectiles["wind_util"].SetActive(false);

            projectiles["lightning_util"].GetComponent<IceBlock>().owner = gameObject;
            projectiles["lightning_util"].SetActive(false);

            projectiles["fire_util"].GetComponent<IceBlock>().owner = gameObject;
            projectiles["fire_util"].SetActive(false);


        }

        //vfx entities
        if (vfxEntities.Count < 1)
        {
            vfxEntities.Add("spell_charge", Instantiate(vfxList[0]));



            vfxEntities["spell_charge"].GetComponent<SpellChargeEntity>().owner = gameObject;
            vfxEntities["spell_charge"].SetActive(false);


        }

    }

    void DisableAllHitboxes()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
        }

    }

    public void TakeDamage(GameObject hitPlayer, int damage, int xKnockback, int yKnockback, int hitstun)
    {

        //If this player is block and facing the right direction
        if (state == PlayerState.Shield &&
            ((hitPlayer.transform.position.x > gameObject.transform.position.x && facingRight) ||
            (hitPlayer.transform.position.x < gameObject.transform.position.x && !facingRight)))
        {
            hspd = xKnockback / 2;
        }
        else
        {
            health -= damage;
            hspd = xKnockback;
            vspd = yKnockback;
            hitstunVal = hitstun;
            SetState(PlayerState.Hitstun);
        }

        if(health <= 0)
        {
            Die();
        }
        Debug.Log("Player Health: " + health);


    }
    public void Die()
    {
        //disable player
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", .3f);
        //gameObject.GetComponent<InputHandler>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        isAlive = false;
        //this is where we would do death burst animations


    }

    public void Respawn()
    {
        if (health <= 0)
        {
            GameManager.Instance.stockCount--;
        }
        if (GameManager.Instance.stockCount <= 0)
        {
            Debug.Log("DEAD BOY ALERT DEADYDEAD BOY OVER HERE");
            //return;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 1f);
            //gameObject.GetComponent<InputHandler>().enabled = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<Rigidbody2D>().simulated = true;
            isAlive = true;
            health = 10;

        }
        //reset player
        //gameObject.SetActive(true);

        //this respawn point should be set based on the map
        //gameObject.transform.position = Vector3.zero;
        hspd = 0;
        vspd = 0;
        SetState(PlayerState.Idle);
    }

    private void CycleWeapon()
    {
        if (otherWeaponAnimControllers.Count == 0) return;

        currentAnimControllerIndex++;
        if (currentAnimControllerIndex > otherWeaponAnimControllers.Count)
        {
            currentAnimControllerIndex = 0;
        }

        if (currentAnimControllerIndex == 0)
        {
            animator.runtimeAnimatorController = baseAnimController;
            weaponName = "ice";
            animator.SetInteger(name: "player_state", (int)PlayerState.Menuing);
        }
        else
        {
            animator.runtimeAnimatorController = otherWeaponAnimControllers[currentAnimControllerIndex - 1];
            weaponName = otherWeaponAnimControllers[currentAnimControllerIndex - 1].name;
        }

        InitWeapon();

    }

    private void CycleColor()
    {
        currentColorIndex++;
        if (currentColorIndex >= colorPalletes.Count)
        {
            currentColorIndex = 0;
        }
        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", colorPalletes[currentColorIndex]);

    }
}
