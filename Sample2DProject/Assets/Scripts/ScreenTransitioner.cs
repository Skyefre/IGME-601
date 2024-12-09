using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ScreenTransitioner : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;
    private bool isLoading = false;

    void Update()
    {
        
    }

    

    public void ExitLoad()
    {
        transition.SetTrigger("End");
    }
    public void EnterLoad(string levelName)
    {
        if (!isLoading)
        {
            StartCoroutine(EnterLoadScene(levelName));
        }
    }

    IEnumerator EnterLoadScene(string levelName)
    {
        isLoading = true;
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelName);

        transition.SetTrigger("End");
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
        {
            GameManager.Instance.players[i].GetComponent<Player>().hspd = 0;
            GameManager.Instance.players[i].GetComponent<Player>().vspd = 0;
            GameManager.Instance.players[i].GetComponent<Player>().Respawn();
            GameManager.Instance.players[i].GetComponent<Animator>().enabled = true;
            //GameManager.Instance.players[i].GetComponent<PlayerInput>().enabled = true;
        }

        //set up special positions for results screen
        if (levelName == "Scene_MainMenu")
        {
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                GameManager.Instance.stockCount = 5;//the reason there are 2 stock sets is so the player actually respawns, then gets set to max stocks
                GameManager.Instance.players[i].GetComponent<Player>().Respawn();
                GameManager.Instance.stockCount = 5;
                GameManager.Instance.players[i].transform.position = new Vector3(0, -64, 0); ;
                GameManager.Instance.players[i].GetComponent<Player>().ResetBoxCollider();
            }
        }
        else if (levelName == "TestScene")
        {
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                GameManager.Instance.players[i].transform.position = new Vector3(-2664f, -111.1f);
                GameManager.Instance.players[i].GetComponent<Player>().ResetBoxCollider();
            }
        }
        else if (levelName == "FinalLevel")
        {
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                GameManager.Instance.players[i].transform.position = new Vector3(-6674f, -111.1f);
                GameManager.Instance.players[i].GetComponent<Player>().ResetBoxCollider();

            }
            GameManager.Instance.Audio.clip = GameManager.Instance.BattleMusic;
            GameManager.Instance.Audio.Play();
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.players.Length; i++)
            {
                GameManager.Instance.players[i].GetComponent<Player>().Respawn();
                GameManager.Instance.players[i].transform.position = new Vector3(0, 10, 0);
                GameManager.Instance.players[i].GetComponent<Player>().ResetBoxCollider();
            }
        }
        isLoading = false;
    }

}
