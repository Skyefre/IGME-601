using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCamera;
    public float shakeAmount = 0;
    Vector3 tempCamPos;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }


    public void Shake(float amount, float length)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        shakeAmount = amount;
        tempCamPos = mainCamera.transform.position;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    private void Update()
    {
        //if (mainCamera == null)
        //{
        //    mainCamera = Camera.main;
        //}
    }

    void DoShake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

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
            mainCamera.transform.localPosition = tempCamPos;
        }

    }

    void StopShake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        CancelInvoke("DoShake");
        mainCamera.transform.localPosition = tempCamPos;
    }

    public void GetCamera()
    {
        mainCamera = Camera.main;
    }
}
