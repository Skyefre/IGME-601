using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void Restart()
    {
        Reset();
        SceneManager.LoadScene("TestScene");
    }

    private void Reset()
    {
        GameManager.Instance.stockCount = 7;
        foreach (GameObject player in GameManager.Instance.players)
        {
            player.gameObject.GetComponent<Player>().isAlive = true;
            player.gameObject.GetComponent<Player>().transform.position = new Vector3(-2664f, -111.1f);
            player.gameObject.GetComponent<Player>().Respawn();
        }
    }
}
