using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public GameObject owner;
    public bool hurtboxActive;
    public int xoffset;
    public int yoffset;
    public int width;
    public int height;

    BoxCollider2D hurtCollider;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        hurtCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void updateHurtbox( int xoffset, int yoffset, int width, int height)
    {
        this.xoffset = xoffset;
        this.yoffset = yoffset;
        this.width = width;
        this.height = height;

        if (owner.GetComponent<Player>() != null)
        {
            gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset + (width / 2)) * 2 * (owner.GetComponent<Player>().facingRight ? 1 : -1),
            owner.transform.position.y + (yoffset - (height / 2)) * 2 - 2,
            0);
        }
        else if (owner.GetComponent<Enemy>() != null)
        {
            gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset + (width / 2)) * 2 * (owner.GetComponent<Enemy>().facingRight ? 1 : -1),
            owner.transform.position.y + (yoffset - (height / 2)) * 2 - 2,
            0);
        }
        else
        {
            Debug.Log("Hurtbox owner is not a player or enemy");
        }

        

        gameObject.transform.localScale = new Vector3(width * 2, height * 2, 1);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Hitbox hitHitbox = collision.gameObject.GetComponent<Hitbox>();
        if(hitHitbox == null)
        {
            return;
        }
        Player hitPlayer;
        Enemy hitEnemy;
        GameObject hitboxOwner;

        if(hitHitbox.owner != null)
        {
            if (hitHitbox.isProjectile)
            {
                hitPlayer = hitHitbox.owner.GetComponent<ProjectileBehavior>().owner.GetComponent<Player>();
                hitEnemy = hitHitbox.owner.GetComponent<ProjectileBehavior>().GetComponent<Enemy>();
                hitboxOwner = hitHitbox.owner.GetComponent<ProjectileBehavior>().owner;
            }
            else
            {
                hitPlayer = hitHitbox.owner.GetComponent<Player>();
                hitEnemy = hitHitbox.owner.GetComponent<Enemy>();
                hitboxOwner = hitHitbox.owner;
            }
        }
        else
        {
            hitboxOwner = null;
            hitPlayer = null;
            hitEnemy = null;
        }
        
        if (hitHitbox != null && hitboxOwner != this.owner && (!hitHitbox.ignoreHurtboxes.Contains(owner)) && hitPlayer != null) //the thing that hit you is a player
        {
            //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
            //hitHitbox.hitboxActive = false;
            hitHitbox.ignoreHurtboxes.Add(owner);
            hitHitbox.canCancel = true;
            //if (owner.GetComponent<Player>() != null)
            //{
            //    owner.GetComponent<Player>().TakeDamage(hitHitbox.owner, 0 /*players cant damage eachother*/, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
            //    owner.GetComponent<Player>().hitstopVal = 10;
            //    owner.GetComponent<Player>().animator.enabled = false;
            //}
            /*else*/ if(owner.GetComponent<Enemy>() != null)
            {
                owner.GetComponent<Enemy>().TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
                owner.GetComponent<Enemy>().hitstopVal = 10;
                owner.GetComponent<Enemy>().animator.enabled = false;

                //if the hitbox is a projectile, destroy it
                if (hitHitbox.isProjectile)
                {
                    hitHitbox.owner.GetComponent<ProjectileBehavior>().DestroyProjectile();
                }

                hitPlayer.hitstopVal = 10;
                hitPlayer.animator.enabled = false;
                GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f / 6f)); //shake the camera when hit
            }
            else
            {
                Debug.Log("Hurtbox owner is not a player or enemy");
            }

            ////if the hitbox is a projectile, destroy it
            //if (hitHitbox.isProjectile)
            //{
            //    hitHitbox.owner.GetComponent<ProjectileBehavior>().DestroyProjectile();
            //}

            //hitPlayer.hitstopVal = 10;
            //hitPlayer.animator.enabled = false;
            //GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f/6f)); //shake the camera when hit
        }
        else if(hitHitbox != null && hitboxOwner != this.owner && (!hitHitbox.ignoreHurtboxes.Contains(owner)) && hitEnemy != null)//the thing that hit you is an enemy
        {
            //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
            hitHitbox.ignoreHurtboxes.Add(owner);
            hitHitbox.canCancel = true;
            if (owner.GetComponent<Player>() != null)
            {
                owner.GetComponent<Player>().TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitEnemy.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
                owner.GetComponent<Player>().hitstopVal = 10;
                owner.GetComponent<Player>().animator.enabled = false;
            }
            else if (owner.GetComponent<Enemy>() != null)
            {
                owner.GetComponent<Enemy>().TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitEnemy.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
                owner.GetComponent<Enemy>().hitstopVal = 10;
                owner.GetComponent<Enemy>().animator.enabled = false;
            }
            else
            {
                Debug.Log("Hurtbox owner is not a player or enemy");
            }
            hitEnemy.hitstopVal = 10;
            hitEnemy.animator.enabled = false;

            //if the hitbox is a projectile, destroy it
            if (hitHitbox.isProjectile)
            {
                hitHitbox.owner.GetComponent<ProjectileBehavior>().DestroyProjectile();
            }
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f/6f)); //shake the camera when hit
        }
        else if (hitHitbox != null && hitboxOwner == null && (!hitHitbox.ignoreHurtboxes.Contains(owner))) //If the hitbox has no owner (e.g. spikes)
        {
            if (owner.GetComponent<Player>() != null)
            {
                owner.GetComponent<Player>().TakeDamage(owner, hitHitbox.damage, -2 * (owner.GetComponent<Player>().facingRight ? -2 : 2), 15, 15);
                owner.GetComponent<Player>().hitstopVal = 10;
                owner.GetComponent<Player>().animator.enabled = false;
            }
            else if (owner.GetComponent<Enemy>() != null)
            {
                owner.GetComponent<Enemy>().TakeDamage(owner, hitHitbox.damage, -2 * (owner.GetComponent<Enemy>().facingRight ? 1 : -1), 10, 15);
                owner.GetComponent<Enemy>().hitstopVal = 10;
                owner.GetComponent<Enemy>().animator.enabled = false;
            }
            else
            {
                Debug.Log("Hurtbox owner is not a player or enemy");
            }
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f / 6f)); //shake the camera when hit
        }
    }
}

