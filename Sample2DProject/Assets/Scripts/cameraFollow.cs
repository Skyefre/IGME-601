using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class cameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    [SerializeField] private float minZoom = 180f;
    [SerializeField] private float maxZoom = 360f;
    [SerializeField] private float minDistance = 360f;
    [SerializeField] private float zoomLimiter = 960f;

    public Vector3 target;
    private Vector3 vel = Vector3.zero;
    private Camera cam;
    private PixelPerfectCamera pixelPerfectCamera;

    private void Start()
    {
        cam = GetComponent<Camera>();
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.players.Length > 0)
        {
            // Get the average position of all players and set the camera to that position
            Vector3 averagePos = Vector3.zero;
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                averagePos += GameManager.Instance.players[i].transform.position;
            }
            averagePos /= GameManager.Instance.players.Length;
            target = averagePos + offset;

            // Calculate the new zoom level based on the distance between players
            float greatestDistance = GetGreatestDistance();
            float newZoom = minZoom;

            if (greatestDistance > minDistance)
            {
                newZoom = Mathf.Lerp(minZoom, maxZoom, (greatestDistance - minDistance) / zoomLimiter);
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);

            // Disable Pixel Perfect Camera if zoomed out
            pixelPerfectCamera.enabled = cam.orthographicSize <= minZoom;
        }

        target.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, damping);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(GameManager.Instance.players[0].transform.position, Vector3.zero);
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
        {
            bounds.Encapsulate(GameManager.Instance.players[i].transform.position);
        }
        return bounds.size.magnitude;
    }
}
