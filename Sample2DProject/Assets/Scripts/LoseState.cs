using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LoseState : MonoBehaviour
{
    public int stocks;
    public int playerCount;

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckLoss();
    }

    private void CheckLoss()
    {
        stocks = GameManager.Instance.stockCount;
        playerCount = GameManager.Instance.players.Length;

        if (stocks <= 0)
        {
            foreach (GameObject player in GameManager.Instance.players)
            {
                if (player.GetComponent<BoxCollider2D>().enabled == false)
                {
                    Debug.Log("Player's defeated!");
                    GameManager.Instance.LoadScene("Lose");
                }
            }
        }
    }
}
