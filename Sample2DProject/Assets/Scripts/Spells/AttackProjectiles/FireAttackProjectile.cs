using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackProjectile : ProjectileBehavior
{
    private float tempHspd = 0;
    private float tempVspd = 0;
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        transform.position += new Vector3(tempHspd, tempVspd, 0);

        //destroy projectile after lifetime
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            //DestroyProjectile();
            GetComponent<Animator>().SetTrigger("explode");
            tempHspd = 0;
            tempVspd = 0;
            hitbox.SetActive(true);
        }

        //destroy projectile after lifetime
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("fire_attack"))
        {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                DestroyProjectile();
            }
        }
        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
    }

    public override void InitProjectile(int spawnX, int spawnY)
    {
        base.InitProjectile(spawnX, spawnY);
        tempHspd = hspd;
        tempVspd = vspd;
        hitbox.SetActive(false);

    }
}
