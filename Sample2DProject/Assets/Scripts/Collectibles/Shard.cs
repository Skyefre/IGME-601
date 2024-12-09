using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Shard : MonoBehaviour, ICollectible
{
    public static int shardsCount = 0;
    public TextMeshProUGUI shardCountText;
    public ShardCounter shardCounter;
    public static event Action OnShardCollected;

    void Start()
    {
        if (shardCountText == null)
        {
            shardCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        }
        if(shardCounter == null)
        {
            shardCounter = GameObject.Find("ShardCounter").GetComponent<ShardCounter>();
        }
    }

    public void Collect()
    {
        GameManager.Instance.ShardsCollected++;
        shardsCount++;
        shardCounter.AddShard();
        shardCountText.text = shardsCount.ToString() + "/4";
        Debug.Log("You collected an item!");
        Destroy(gameObject);
        OnShardCollected?.Invoke();
    }

}
