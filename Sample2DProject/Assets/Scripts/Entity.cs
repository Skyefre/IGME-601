using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class Entity : MonoBehaviour
{

    //entity speed
    public float hspd = 0.0f;
    public float vspd = 0.0f;
    public float lifeTime = 5.0f;
    public GameObject owner;
    public bool facingRight = true;
    //public GameObject hitbox;
    public int damage = 1;
    public int xoffset;
    public int yoffset;
    public int width;
    public int height;
    public int xKnockback;
    public int yKnockback;
    public int hitstun;
    public bool disableAfterAnimation = false;
    public bool projectileActive = true;

    protected float timer = 0.0f;

    protected void Start()
    {
        //InitProjectile(0,0);
    }
    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        transform.position += new Vector3(hspd, vspd, 0);

        if (disableAfterAnimation)
        {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                DestroyEntity();
            }
        }
        else
        {
            //destroy projectile after lifetime
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                DestroyEntity();
            }
        }

        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
    }

    
    public virtual void DestroyEntity()
    {
        timer = 0;
        //if (hitbox != null)
        //{
        //    hitbox.GetComponent<Hitbox>().hitboxActive = false;
        //}

        gameObject.SetActive(false);
    }



    public virtual void InitEntity(int spawnX, int spawnY)
    {
        if (owner == null)
        {
            Debug.Log("entity owner not set");
            return;
        }
        facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : true;
        gameObject.GetComponent<Animator>().SetTrigger("restart");
        //if (hitbox != null)
        //{
        //    hitbox.GetComponent<Hitbox>().owner = gameObject;
        //    hitbox.GetComponent<Hitbox>().isProjectile = true;
        //    hitbox.GetComponent<Hitbox>().hitboxActive = true;
        //    hitbox.GetComponent<Hitbox>().updateHitbox(damage, 0, 0, width, height, xKnockback, yKnockback, hitstun);
        //}


        hspd = Mathf.Abs(hspd) * (facingRight ? 1 : -1);
        gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
        DontDestroyOnLoad(gameObject);
    }
    public virtual void InitEntity(int spawnX, int spawnY, Texture targetTexture)
    {
        if (owner == null)
        {
            Debug.Log("entity owner not set");
            return;
        }
        facingRight = owner.GetComponent<Player>() != null ? owner.GetComponent<Player>().facingRight : true;
        gameObject.GetComponent<Animator>().SetTrigger("restart");
        //if (hitbox != null)
        //{
        //    hitbox.GetComponent<Hitbox>().owner = gameObject;
        //    hitbox.GetComponent<Hitbox>().isProjectile = true;
        //    hitbox.GetComponent<Hitbox>().hitboxActive = true;
        //    hitbox.GetComponent<Hitbox>().updateHitbox(damage, 0, 0, width, height, xKnockback, yKnockback, hitstun);
        //}

        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", targetTexture);

        hspd = Mathf.Abs(hspd) * (facingRight ? 1 : -1);
        gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
        DontDestroyOnLoad(gameObject);
    }


}
