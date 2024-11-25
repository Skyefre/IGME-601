using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHurtbox : Hurtbox
{
    public string targetElementTag = "Wind";
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void updateHurtbox(int xoffset, int yoffset, int width, int height)
    {
        base.updateHurtbox(xoffset, yoffset, width, height);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        Hitbox hitHitbox = collision.gameObject.GetComponent<Hitbox>();
        if (hitHitbox == null)
        {
            return;
        }
        Player hitPlayer;
        Enemy hitEnemy;
        GameObject hitboxOwner;

        if (hitHitbox.owner != null)
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

        if (hitHitbox.owner.tag != targetElementTag)
        {
            return;
        }

        if (hitHitbox != null && hitboxOwner != this.owner && (!hitHitbox.ignoreHurtboxes.Contains(owner)) && hitPlayer != null) //the thing that hit you is a player
        {
            //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
            hitHitbox.ignoreHurtboxes.Add(owner);
            hitHitbox.canCancel = true;
            if (owner.GetComponent<IceBlock>() != null)
            {
                owner.GetComponent<IceBlock>().TakeKnockback(hitHitbox.owner, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
            }

            //if the hitbox is a projectile, destroy it
            if (hitHitbox.isProjectile)
            {
                hitHitbox.owner.GetComponent<ProjectileBehavior>().DestroyProjectile();
            }

            hitPlayer.hitstopVal = 10;
            hitPlayer.animator.enabled = false;
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f / 6f)); //shake the camera when hit
        }
        else if (hitHitbox != null && hitboxOwner != this.owner && (!hitHitbox.ignoreHurtboxes.Contains(owner)) && hitEnemy != null)//the thing that hit you is an enemy
        {
            //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
            hitHitbox.ignoreHurtboxes.Add(owner);
            hitHitbox.canCancel = true;
            if (owner.GetComponent<IceBlock>() != null)
            {
                owner.GetComponent<IceBlock>().TakeKnockback(hitHitbox.owner, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
            }
            hitEnemy.hitstopVal = 10;
            hitEnemy.animator.enabled = false;

            //if the hitbox is a projectile, destroy it
            if (hitHitbox.isProjectile)
            {
                hitHitbox.owner.GetComponent<ProjectileBehavior>().DestroyProjectile();
            }
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f / 6f)); //shake the camera when hit
        }
        else if (hitHitbox != null && hitboxOwner == null && (!hitHitbox.ignoreHurtboxes.Contains(owner))) //If the hitbox has no owner (e.g. spikes)
        {
            if (owner.GetComponent<IceBlock>() != null)
            {
                owner.GetComponent<IceBlock>().TakeKnockback(hitHitbox.owner, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
            }
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(3f, (1f / 6f)); //shake the camera when hit
        }
    }
}
