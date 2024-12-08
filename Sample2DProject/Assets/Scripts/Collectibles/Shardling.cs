using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Shardling : MonoBehaviour, ICollectible
{
    public static int shardlingCount = 0;
    public TextMeshProUGUI shardlingCountText;
    public TextMeshProUGUI stockCountText;


    public static event Action OnShardlingCollected;

    void Start()
    {
        if (shardlingCountText == null)
        {
            shardlingCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        }
        if(stockCountText == null)
        {
            shardlingCountText = GameObject.Find("StockCountText").GetComponent<TextMeshProUGUI>();

        }
    }

    public void Collect()
    {
        GameManager.Instance.ShardlingsCollected++;
        if(GameManager.Instance.ShardlingsCollected>= 25)
        {
            GameManager.Instance.stockCount++;
            GameManager.Instance.ShardlingsCollected = 0;

        }
        shardlingCount++;
        //shardlingCountText.text = shardlingCount.ToString();
        Debug.Log("You collected a Shardling!");
        Destroy(gameObject);
        OnShardlingCollected?.Invoke();
    }

}
