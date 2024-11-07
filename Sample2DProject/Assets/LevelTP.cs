using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTP : MonoBehaviour
{
    public bool InTeleport;
    public GameManager gameManager;

    private void Start()
    {
        InTeleport = false;
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager.players.Length >= 2)
        {
            InTeleport = true;
            gameManager.LoadScene("TestScene");

        }
        else
        {
            Debug.Log("Not all players in!");

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Portal")
        {
            Debug.Log("teleport not ready!");
            InTeleport = false;

        }
    }
}
