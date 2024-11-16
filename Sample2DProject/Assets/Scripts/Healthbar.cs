using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Animator animator;
    public int playerNumber = 1;
    public Texture2D basePalette;
    //public Texture2D[] colorPalettes;

    // Animation info fields
    public AnimatorStateInfo animStateInfo;
    public AnimatorClipInfo[] currentClipInfo;
    private int currentFrame;
    private int frameCount;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.players.Count >= playerNumber)
        {
            gameObject.GetComponent<Image>().enabled = true;
            Player player = GameManager.Instance.players[playerNumber - 1].GetComponent<Player>();
            //gameObject.GetComponent<Image>().material.SetTexture("_PaletteTex",
            //    GameManager.Instance.colorPalettes[player.currentColorIndex]);

            int stockCount = GameManager.Instance.stockCount;
            animator.SetInteger("StockCount", stockCount > 0? stockCount: 1);

            // Force the animator to update its state
            animator.Update(0);

            // Get animator info after update
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            frameCount = (int)(currentClipInfo[0].clip.length * currentClipInfo[0].clip.frameRate);

            // Set the animator to a specific frame based on stock count
            float normalizedTime = (1f / frameCount) * (8 - player.health);
            animator.Play(currentClipInfo[0].clip.name, 0, normalizedTime);
            animator.speed = 0;
        }
        else
        {
            gameObject.GetComponent<Image>().material.SetTexture("_PaletteTex", basePalette);
            animator.SetInteger("StockCount", 0);
            //If there is no connected player, hide the health bar
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_MainMenu")
            {
                gameObject.GetComponent<Image>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Image>().enabled = true;
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "results")
        {
            gameObject.GetComponent<Image>().enabled = false;
        }
    }

    void SetHealthVal(int targetHealth)
    {
        animator.Update(Time.deltaTime);
        animator.Play(currentClipInfo[0].clip.name, 0, (1f / frameCount) * (8 - targetHealth));
    }
}
