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
    private float newJumpForce;
    private float baseJumpForce;

    //protected float timer = 0.0f;
    protected float abilityTimer = 0.0f;

    private void Awake()
    {
        //save base jump force, change current jump force to new.
        //baseJumpForce = owner.GetComponent<Player>().jumpForce;
        //newJumpForce = baseJumpForce * 2;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        transform.position += new Vector3(hspd, vspd, 0);

        //decrease player's gravity and increase their jumpheight for 5 seconds
        abilityTimer += Time.deltaTime;

        //set jump force to new 
        //owner.GetComponent<Player>().jumpForce = newJumpForce;

        //restore player stats when timer is up
        if (abilityTimer >= lifeTime)
        {
            RestoreStats();
        }

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
        //hitbox.GetComponent<Hitbox>().hitboxActive = false;
        gameObject.SetActive(false);
    }

    //restore player stats to original values
    public void RestoreStats()
    {
        //reset timer
        abilityTimer = 0;

        //reset values
        //player.gravity *= 2;
        owner.GetComponent<Player>().jumpForce = baseJumpForce;
    }

    //spawn the wind blast in front of the player
    public override void InitProjectile(int spawnX, int spawnY)
    {
        if (owner == null)
        {
            Debug.Log("Wind Util owner not set");
            return;
        }
        facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : (owner.GetComponent<Enemy>() != null ? owner.GetComponent<Enemy>().facingRight : true);

        gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
    }
}
