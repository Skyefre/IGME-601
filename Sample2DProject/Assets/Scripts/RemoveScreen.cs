using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveScreen : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        gameObject.SetActive(false);
    }
}
