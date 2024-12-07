using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialSpriteSelect : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetInteger("state", state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
