using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

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
    public string weaponName = "ice";
    public Animator animator;
    //public AnimatorController baseAnimController;
    public RuntimeAnimatorController baseAnimController;
    public List<AnimatorOverrideController> otherWeaponAnimControllers;
    public List<Texture2D> colorPalletes;
    public JSONReader characterJSON;
    public JSONReader.FrameDataContainer frameData;
    public JSONReader.HitboxDataContainer hitboxData;
    public JSONReader.HurtboxDataContainer hurtboxData;
    public int maxHitboxes = 2;
    private JSONReader.WeaponDataList weaponData;
    private int currentAnimControllerIndex = 0;
    private int currentColorIndex = 0;

    //Enemy fields
    public int runSpeed = 3;
    public int jumpForce = 10;
    public int gravity = 1;
    public int health = 100;
    public int hspd = 0;
    public int vspd = 0;
    public int maxHspd = 10;
    public int maxVspd = 1;
    public EnemyState state = EnemyState.Idle;
    public BaseSpell currentSpell;
    public BaseSpell[] PlayerSpells;
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

    public BoxCollider2D PlayerCollider
    {
        get => boxCollider;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
