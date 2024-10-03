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
    void Start()
    {
        hurtCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateHurtbox( int xoffset, int yoffset, int width, int height)
    {
        this.xoffset = xoffset;
        this.yoffset = yoffset;
        this.width = width;
        this.height = height;

        gameObject.transform.position = new Vector3(
            owner.transform.position.x + (xoffset + (width / 2)) * 2 * (owner.GetComponent<Player>().facingRight ? 1 : -1),
            owner.transform.position.y + (yoffset - (height / 2)) * 2-2,
            0);

        gameObject.transform.localScale = new Vector3(width * 2, height * 2, 1);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Hitbox hitHitbox = collision.gameObject.GetComponent<Hitbox>();
        Player hitPlayer = hitHitbox.owner.GetComponent<Player>();
        if (hitHitbox != null && hitHitbox.owner != this.owner && hitHitbox.hitboxActive == true && hitPlayer != null)
        {
            //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
            hitHitbox.hitboxActive = false;
            hitHitbox.canCancel = true;
            owner.GetComponent<Player>().TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun);
            owner.GetComponent<Player>().hitstopVal = 6;
            owner.GetComponent<Player>().animator.enabled = false;
            hitPlayer.hitstopVal = 6;
            hitPlayer.animator.enabled = false;
            GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(1f, .1f); //shake the camera when hit
        }
    }
}
