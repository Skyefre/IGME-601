using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    //variables (commented out ones aren't necessary right now but I believe we'll need them for
    public float hspd = 0.0f;
    public float vspd = 0.0f;
    public float lifeTime = 5.0f;
    public GameObject owner;
    public bool facingRight = true;
    public GameObject hitbox;
    //public int damage = 1;
    public int xoffset;
    public int yoffset;
    public int width;
    public int height;
    //public int xKnockback;
    //public int yKnockback;
    //public int hitstun;

    //so that the block only persists a certain amount of time
    private float timer = 0.0f;

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = new Vector3(hspd, vspd, 0);

        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            DestroyBlock();
        }
    }

    //destroy the block
    public void DestroyBlock()
    {
        timer = 0;
        hitbox.GetComponent<Hitbox>().hitboxActive = false;
        gameObject.SetActive(false);
    }

    //spawn a block in front of the player
    public void InitBlock(int spawnX, int spawnY)
    {
        if (owner == null)
        {
            Debug.Log("Block owner not set");
            return;
        }
        gameObject.transform.position = new Vector3(owner.transform.position.x + spawnX * (facingRight ? 1 : -1), owner.transform.position.y + spawnY, 0);
    }
}
