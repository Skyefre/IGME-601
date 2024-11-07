using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static Player;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager playerInputManager;
    public GameObject[] players;
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

 

        if (players.Length>=2)
        {

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                RespawnPlayer();
            }
            
        }
       
    }
    public void LoadScene(string sceneName)
    {
        for (int i = 0; i < players.Length; i++)
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
            for (int i = 0; i < players.Length; i++)
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
        players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject player in players)
        //{
        //    Debug.Log(player.name);
        //}
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", colorPalletes[i]);
        }
    }

    public void RespawnPlayer()
    {
        if(players.Length>1 && PlayerLives>=1) {

            if (players[0].activeInHierarchy == false)
            {

                players[0].SetActive(true);
                players[0].transform.position = players[1].transform.position;
            }
            else
            {

                players[1].SetActive(true);
                players[1].transform.position = players[0].transform.position;
            }
            PlayerLives--;
            Debug.Log("Player Respawned! Player Lives left: " + PlayerLives);

        }
        
        Debug.Log("all players alive");

    }
}
