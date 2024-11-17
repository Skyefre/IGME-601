using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellChargeEntity : ProjectileBehavior
{
    public Animator animator;

    public void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        
    }
    protected override void Start()
    {
        if(owner != null)
        {
            gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", GameManager.Instance.players[0] == owner? GameManager.Instance.colorPalettes[0]: GameManager.Instance.colorPalettes[1]);
        }
    }


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if(owner != null) {
            transform.position = new Vector3(owner.transform.position.x + xoffset, owner.transform.position.y+yoffset,0);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            animator.SetBool("charged", true);
        }
    }

    public override void InitProjectile(int spawnX, int spawnY)
    {
        base.InitProjectile(spawnX, spawnY);
        animator.SetBool("charged", false);
    }
}
