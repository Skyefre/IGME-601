using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void Restart()
    {
        Reset();
        SceneManager.LoadScene("Scene_MainMenu");
    }

    private void Reset()
    {
        GameManager.Instance.stockCount = 5;
        GameManager.Instance.ShardsCollected = 0;
        foreach (GameObject player in GameManager.Instance.players)
        {
            player.gameObject.GetComponent<Player>().isAlive = true;
            player.gameObject.GetComponent<Player>().transform.position = new Vector3(-2664f, -111.1f);
            player.gameObject.GetComponent<Player>().Respawn();
        }
        foreach (GameObject player in GameManager.Instance.players)
        {
            player.gameObject.GetComponent<Player>().transform.position = new Vector3(-2664f, -111.1f);
        }
    }
}
