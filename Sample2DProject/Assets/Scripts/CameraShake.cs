using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCamera;

    public float shakeAmount = 0;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void Shake(float amount, float length)
    {
        shakeAmount = amount;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        Shake(1f, 1f);
    //    }
    //}

    void DoShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = mainCamera.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCamera.transform.position = camPos;
        }
        else
        {
            CancelInvoke("DoShake");
            mainCamera.transform.localPosition = new Vector3(0,0,-10);
        }

    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        mainCamera.transform.localPosition = new Vector3(0, 0, -10);
    }
}
