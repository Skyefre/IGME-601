using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] players;
    public List<Texture2D> colorPalettes;
    public bool enableStartScreen;
    public int ShardsCollected;
    public int ShardlingsCollected;
    public int stockCount;
    public ScreenTransitioner screenTransitioner;
    public List<string> p1Shards;
    public List<string> p2Shards;
    public List<string> globalShardList;

    private Dictionary<InputHandler.Inputs, InputHandler.InputState> inputs;

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
        ShardsCollected = 0;
        ShardlingsCollected = 0;
        stockCount = 5;
    }


    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_MainMenu")
        {
            //CheckAlivePlayers();
            gameObject.GetComponent<PlayerInputManager>().DisableJoining();
        }
        else
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadScene("TestScene");
            }
            gameObject.GetComponent<PlayerInputManager>().EnableJoining();
        }

        if (Input.GetKey(KeyCode.R))
        {
            LoadScene("Scene_MainMenu");
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lose" || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Win")
        {
            //for (int i = 0; i < GameManager.Instance.players.Length; i++)
            if (Input.anyKey)
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
                LoadScene("Scene_MainMenu");
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

            foreach (KeyValuePair<string, GameObject> vfxEntity in players[i].GetComponent<Player>().vfxEntities)
            {
                DontDestroyOnLoad(vfxEntity.Value);
            }
        }
        screenTransitioner.EnterLoad(sceneName);
        

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
            players[i].GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", colorPalettes[i]);
            players[i].GetComponent<Player>().playerNumber = i+1;
        }
    }

    public void DebugTP()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = new Vector3(-2664f, -111.1f);
        }
    }
}
