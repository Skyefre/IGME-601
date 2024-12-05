using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class cameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    [SerializeField] private float minZoom = 180f;
    [SerializeField] private float maxZoom = 1280F;
    [SerializeField] private float minDistance = 360f;
    [SerializeField] private float zoomLimiter = 960f;
    [SerializeField] private float zoomSpeed = 1f;

    public Vector3 target;
    private Vector3 vel = Vector3.zero;
    private Camera cam;
    private PixelPerfectCamera pixelPerfectCamera;
    private bool camInitialized = false;
    private bool cameraClamped = false;

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
            int numAlivePlayers = 0;
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                if (GameManager.Instance.players[i].GetComponent<Player>().isAlive)
                {
                    averagePos += GameManager.Instance.players[i].transform.position;
                    numAlivePlayers++;
                }
            }
            if (numAlivePlayers == 0)
            {
                return;
            }
            averagePos /= numAlivePlayers;
            target = averagePos + offset;

            // Calculate the new zoom level based on the distance between players
            Bounds greatestDistance = GetGreatestDistance();
            float newZoom = minZoom;

            if (greatestDistance.size.x > minDistance || greatestDistance.size.y > (minDistance / 16 * 9))
            {
                if (greatestDistance.size.x >= greatestDistance.size.y)
                {
                    newZoom = Mathf.Lerp(minZoom, maxZoom, ((greatestDistance.size.x / 16 * 9) - (minDistance / 16 * 9)) / zoomLimiter);
                }
                else
                {
                    newZoom = Mathf.Lerp(minZoom, maxZoom, (greatestDistance.size.y - (minDistance / 16 * 9)) / zoomLimiter);
                }
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, zoomSpeed * Time.deltaTime);

            // Disable Pixel Perfect Camera if zoomed out
            if (cam.orthographicSize <= minZoom + 2)
            {
                cam.orthographicSize = minZoom;
                pixelPerfectCamera.enabled = true;
            }
            else
            {
                pixelPerfectCamera.enabled = false;
            }

            if(!camInitialized)
            {

                transform.position = new Vector3(target.x, target.y, transform.position.z);
                camInitialized = true;
            }

            // Clamp player positions to the camera bounds
            ClampPlayerPositionsToCameraBounds();
            if(cameraClamped)
            {
                target = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }
        }

        target.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, damping);
    }

    private Bounds GetGreatestDistance()
    {
        var bounds = new Bounds(GameManager.Instance.players[0].transform.position, Vector3.zero);
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
        {
            if (GameManager.Instance.players[i].GetComponent<Player>().isAlive)
            {
                bounds.Encapsulate(GameManager.Instance.players[i].transform.position);
            }
        }
        return bounds;
    }

    private void ClampPlayerPositionsToCameraBounds()
    {
        float cameraHeight = cam.orthographicSize * 2;
        float cameraWidth = cameraHeight * cam.aspect;

        float minX = transform.position.x - cameraWidth / 2;
        float maxX = transform.position.x + cameraWidth / 2;
        float minY = transform.position.y - cameraHeight / 2;
        float maxY = transform.position.y + cameraHeight / 2;

        cameraClamped = true;
        foreach (var player in GameManager.Instance.players)
        {
            if (player.GetComponent<Player>().isAlive)
            {
                Vector3 playerPos = player.transform.position;
                if(!(playerPos.x <= minX+1 || playerPos.x >= maxX-1))
                {
                    cameraClamped = false;
                }
                playerPos.x = Mathf.Clamp(playerPos.x, minX, maxX);
                //playerPos.y = Mathf.Clamp(playerPos.y, minY, maxY);
                player.transform.position = playerPos;


            }
        }
    }
}
