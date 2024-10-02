using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public GameObject owner;
    public bool hitboxActive;
    public bool canCancel;
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

        gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset + (width/2))*2 * (owner.GetComponent<Player>().facingRight ?1:-1),
            owner.transform.position.y + (yoffset - (height/2))*2,
            0);

        gameObject.transform.localScale = new Vector3(width*2, height*2, 1);
    }

}
