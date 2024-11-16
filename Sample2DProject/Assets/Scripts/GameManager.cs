using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static Player;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager playerInputManager;
    public List<GameObject> players;
    public List<Texture2D> colorPalletes;
    public bool enableStartScreen;
    public int ShardsCollected;
    public int FleckCollected;
    public int PlayerLives;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        enableStartScreen = true;


    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Shards initiated");
        FleckCollected = 0;
        PlayerLives = 3;
        ShardsCollected = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_MainMenu")
        {
            //CheckAlivePlayers();
        }
        else
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadScene("TestScene");
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            LoadScene("Scene_MainMenu");
        }



        /*if (players.Length >= 2 && playerInputManager.joiningEnabled==false)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame && players[i].GetComponent<PlayerInput>().currentControlScheme == "Keyboard")
                {
                    if (players[i].GetComponent<Player>().dead)
                    {
                        Debug.Log(players[i].GetComponent<Player>().dead);
                        RespawnPlayer(players[i]);
                    }
                }
                else if (Gamepad.current.startButton.wasPressedThisFrame && players[i].GetComponent<PlayerInput>().currentControlScheme == "Controller")
                {
                    if (players[i].GetComponent<Player>().dead)
                    {
                        Debug.Log(players[i].GetComponent<Player>().dead);
                        RespawnPlayer(players[i]);
                    }
                }
            }

            *//*if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                RespawnPlayer();
            }*//*

        }*/

    }
    public void LoadScene(string sceneName)
    {
        for (int i = 0; i < players.Count; i++)
        {
            DontDestroyOnLoad(players[i]);
            foreach (KeyValuePair<string, GameObject> projectile in players[i].GetComponent<Player>().projectiles)
            {
                DontDestroyOnLoad(projectile.Value);
            }
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        if (sceneName == "TestScene")
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = new Vector3(-2664f, -111.1f);
            }
        }
        Instance.GetComponent<PlayerInputManager>().DisableJoining();
        //gameObject.GetComponent<CameraShake>().mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void GetPlayerIds()
    {
         //players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        //foreach (GameObject player in players)
        //{
        //    Debug.Log(player.name);
        //}
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", colorPalletes[i]);
        }
    }

    public void RespawnPlayer(GameObject player)
    {

        Player playerScript = player.GetComponent<Player>();
        playerScript.dead = false;
        playerScript.gravity = 1;
        playerScript.health = 2;
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<SpriteRenderer>().enabled = true;
        
        players.Add(player);
        players[1].transform.position = players[0].transform.position;
        /* int playerloc = System.Array.IndexOf(players, player);
         if (playerloc == 1)
         {
             otherplayer = players[0];
             player.transform.position = otherplayer.transform.position;

         }
         else
         {
             otherplayer = players[1];
             player.transform.position = otherplayer.transform.position;

         }
         PlayerLives--;*/
        Debug.Log("Player Respawned! Player Lives left: " + PlayerLives);



    }
}
