using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour{

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Vector3 target;
    private Vector3 vel = Vector3.zero;

    private void FixedUpdate()
    {
        if(GameManager.Instance.players.Length > 0)
        {
            //get the average position of all players and set the camera to that position
            Vector3 averagePos = Vector3.zero;
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                averagePos += GameManager.Instance.players[i].transform.position;
            }
            averagePos /= GameManager.Instance.players.Length;
            target = averagePos + offset;
        }
        //Vector3 targetPos = target.position + offset;
        target.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, damping);

    }

}
