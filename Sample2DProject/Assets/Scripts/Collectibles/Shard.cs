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

    public static event Action OnShardCollected;

    void Start()
    {
        if (shardCountText == null)
        {
            shardCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        }
    }

    public void Collect()
    {
        shardsCount++;
        shardCountText.text = shardsCount.ToString() + "/10";
        Debug.Log("You collected an item!");
        Destroy(gameObject);
        OnShardCollected?.Invoke();
    }

}
