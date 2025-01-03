using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public GameObject owner;
    //public bool hitboxActive;
    public List<GameObject> ignoreHurtboxes = new List<GameObject>();
    public bool canCancel;
    public bool isProjectile = false;
    public int damage = 1;
    public int xoffset;
    public int yoffset;
    public int width;
    public int height;
    public int xKnockback;
    public int yKnockback;
    public int hitstun;

    BoxCollider2D hitCollider;
    // Start is called before the first frame update
    void Start()
    {
        hitCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateHitbox(int damage, int xoffset, int yoffset, int width, int height, int xKnockback, int yKnockback, int hitstun)
    {
        this.damage = damage;
        this.xoffset = xoffset;
        this.yoffset = yoffset;
        this.width = width;
        this.height = height;
        this.xKnockback = xKnockback;
        this.yKnockback = yKnockback;
        this.hitstun = hitstun;

        bool facingRight = owner.GetComponent<Player>() != null? owner.GetComponent<Player>().facingRight: (owner.GetComponent<Enemy>() != null ? owner.GetComponent<Enemy>().facingRight : true);

        //if the owner is a projectile, use the projectile's facing direction and skip all the funky math about projectile offsets and scaling
        if (owner.GetComponent<ProjectileBehavior>() != null)
        {
            facingRight = owner.GetComponent<ProjectileBehavior>().facingRight;
            gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset) * (facingRight ? 1 : -1),
            owner.transform.position.y + (yoffset),
            0);
            return;
        }

        //if the owner is a player, use the player's facing direction and normal offsets and scaling
        gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset + (width/2))*2 * (facingRight ?1:-1),
            owner.transform.position.y + (yoffset - (height/2))*2,
            0);

        gameObject.transform.localScale = new Vector3(width*2, height*2, 1);
    }

}
