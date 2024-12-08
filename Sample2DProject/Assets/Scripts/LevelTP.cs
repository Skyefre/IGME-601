using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTP : MonoBehaviour
{
    //public bool InTeleport;
    //public GameManager gameManager;

    //private void Start()
    //{
    //    InTeleport = false;
    //    gameManager = FindObjectOfType<GameManager>();
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Hello!");
    //    Debug.Log(collision.tag);
    //    gameManager = FindObjectOfType<GameManager>();
    //    if (gameManager.players.Length >= 2)
    //    {
    //        InTeleport = true;
    //        gameManager.LoadScene("TestScene");

    //    }
    //    else
    //    {
    //        Debug.Log("Not all players in!");

    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.tag == "Portal")
    //    {
    //        Debug.Log("teleport not ready!");
    //        InTeleport = false;

    //    }
    //}
    public List<GameObject> readyPlayers = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            CheckLevelComplete();
        }
        else
        {
            CheckGameStart();

        }
    }

    private void CheckLevelComplete()
    {
        if (GameManager.Instance.players.Length > 1)
        {
            bool allPlayersReady = true;
            foreach (GameObject player in GameManager.Instance.players)
            {
                if (!readyPlayers.Contains(player))
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                Debug.Log("Wahoo");
                Debug.Log(GameManager.Instance.ShardsCollected);
                if (GameManager.Instance.ShardsCollected >= 4)
                {
                    SceneManager.LoadScene("Win");
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
        {
            if (collision.gameObject == GameManager.Instance.players[i])
            {
                readyPlayers.Add(collision.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
        {
            if (collision.gameObject == GameManager.Instance.players[i])
            {
                readyPlayers.Remove(collision.gameObject);
            }
        }
    }

    

    public void CheckGameStart()
    {
        if (GameManager.Instance.players.Length > 1)
        {
            bool allPlayersReady = true;
            foreach (GameObject player in GameManager.Instance.players)
            {
                if (!readyPlayers.Contains(player))
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                GameManager.Instance.LoadScene("FinalLevel"); 
            }
        }
    }
}
