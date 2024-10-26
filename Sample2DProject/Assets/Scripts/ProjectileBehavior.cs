using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{

    //projectile speed
    public float speed = 3.0f;

    // Update is called once per frame
    private void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }

    //destroy projectile upon collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }


}
