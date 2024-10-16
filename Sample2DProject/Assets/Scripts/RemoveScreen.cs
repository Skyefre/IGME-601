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
        if (GameManager.Instance.enableStartScreen)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Remove()
    {
        if (gameObject.activeSelf)
        {
            GameManager.Instance.enableStartScreen = false;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        gameObject.SetActive(false);
    }
}
