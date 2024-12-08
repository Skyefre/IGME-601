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
            shardlingCountText = GameObject.Find("ShardlingCount").GetComponent<TextMeshProUGUI>();
        }
        if(stockCountText == null)
        {
            stockCountText = GameObject.Find("StockCountText").GetComponent<TextMeshProUGUI>();

        }
    }

    public void Collect()
    {
        Debug.Log(shardlingCountText);
        GameManager.Instance.ShardlingsCollected++;
        shardlingCount++;
        shardlingCountText.text = shardlingCount.ToString();
        Debug.Log("You collected a Shardling!");
        if (GameManager.Instance.ShardlingsCollected >= 25)
        {
            GameManager.Instance.stockCount++;
            GameManager.Instance.ShardlingsCollected = 0;
            shardlingCount = 0;
            shardlingCountText.text = shardlingCount.ToString();

        }
        Destroy(gameObject);
        OnShardlingCollected?.Invoke();
    }

}
