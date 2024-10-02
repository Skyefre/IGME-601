using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.anyKey)
        //{
        //    gameObject.SetActive(false);
        //}
    }
    public void Remove()
    {
        gameObject.SetActive(false);
    }
}
