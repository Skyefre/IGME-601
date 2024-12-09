using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindUtility : ProjectileBehavior
{
    //all the vars this child has access to

    //projectile speed
    //public float hspd = 3.0f;
    //public float vspd = 0.0f;
    //public float lifeTime = 5.0f;
    //public GameObject owner;
    //public bool facingRight = true;
    //public GameObject hitbox;
    //public int damage = 1;
    //public int xoffset;
    //public int yoffset;
    //public int width;
    //public int height;
    //public int xKnockback;
    //public int yKnockback;
    //public int hitstun;
    //public bool projectileActive = true;
    //private float newGravity;
    //private float baseGravity;
    //public BoxCollider2D windCollider;

    //protected float timer = 0.0f;
    protected float abilityTimer = 0.0f;

    //collision
    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    private RaycastHit2D rayRight;
    private RaycastHit2D rayLeft;
    public int iceBlkHspd = 2;

    protected override void Start()
    {
        //save base jump force, change current jump force to new.
        //baseGravity = owner.GetComponent<Player>().gravity;
        //newGravity = 0.75f;
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        transform.position += new Vector3(hspd, vspd, 0);

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
        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
    }

    //destroy the Wind Util
    public override void DestroyProjectile()
    {
        timer = 0;
        
        hitbox.GetComponent<Hitbox>().ignoreHurtboxes.Clear();
        
        gameObject.SetActive(false);
    }

    //restore player stats to original values
    //public void RestoreStats()
    //{
    //    //reset timer
    //    abilityTimer = 0;

    //    //reset values
    //    //player.gravity *= 2;
    //    owner.GetComponent<Player>().gravity = baseGravity;
    //}

    //spawn the wind blast in front of the player
    public override void InitProjectile(int spawnX, int spawnY)
    {
        //if (owner == null)
        //{
        //    Debug.Log("Wind Util owner not set");
        //    return;
        //}
        //projectileActive = true;
        //facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : (owner.GetComponent<Enemy>() != null ? owner.GetComponent<Enemy>().facingRight : true);
        //if (hitbox != null)
        //{
        //    hitbox.GetComponent<Hitbox>().owner = gameObject;
        //    hitbox.GetComponent<Hitbox>().isProjectile = true;
        //    hitbox.GetComponent<Hitbox>().ignoreHurtboxes.Clear();
        //    hitbox.GetComponent<Hitbox>().updateHitbox(damage, 0, 0, width, height, xKnockback, yKnockback, hitstun);
        //}

        //hspd = Mathf.Abs(hspd) * (facingRight ? 1 : -1);
        //gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
        //DontDestroyOnLoad(gameObject);
        base.InitProjectile(spawnX, spawnY);
    }
}
