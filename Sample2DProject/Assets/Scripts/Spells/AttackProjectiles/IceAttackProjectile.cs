using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAttackProjectile : ProjectileBehavior
{

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

    //protected float timer = 0.0f;

    //protected void Start()
    //{
    //    //InitProjectile(0,0);
    //}
    // Update is called once per frame
    //protected override void FixedUpdate()
    //{
    //    transform.position += new Vector3(hspd,vspd,0);

    //    //destroy projectile after lifetime
    //    timer += Time.deltaTime;
    //    if (timer >= lifeTime)
    //    {
    //        DestroyProjectile();
    //    }
    //    gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
    //}

    ////destroy projectile upon collision
    //public override void DestroyProjectile()
    //{
    //    timer = 0;
    //    hitbox.GetComponent<Hitbox>().hitboxActive = false;
    //    gameObject.SetActive(false);
    //}



    //public virtual void InitProjectile(int spawnX, int spawnY)
    //{
    //    if(owner == null)
    //    {
    //        Debug.Log("Projectile owner not set");
    //        return;
    //    }
    //    facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : (owner.GetComponent<Enemy>() != null ? owner.GetComponent<Enemy>().facingRight : true);
    //    hitbox.GetComponent<Hitbox>().owner = gameObject;
    //    hitbox.GetComponent<Hitbox>().isProjectile = true;
    //    hitbox.GetComponent<Hitbox>().hitboxActive = true;
    //    hitbox.GetComponent<Hitbox>().updateHitbox(damage, 0, 0, width, height, xKnockback, yKnockback, hitstun);
    //    hspd = Mathf.Abs(hspd) * (facingRight ? 1 : -1);
    //    gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight?1:-1), owner.transform.position.y + spawnY, 0);
    //    DontDestroyOnLoad(gameObject);
    //}


}
