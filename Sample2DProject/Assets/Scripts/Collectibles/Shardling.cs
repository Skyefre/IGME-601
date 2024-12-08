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

    public static event Action OnShardlingCollected;

    void Start()
    {
       /* if (shardCountText == null)
        {
            shardCountText = GameObject.Find("Count").GetComponent<TextMeshProUGUI>();
        }*/
    }

    public void Collect()
    {
        GameManager.Instance.ShardlingsCollected++;
        shardlingCount++;
        //shardlingCountText.text = shardlingCount.ToString();
        Debug.Log("You collected a Shardling!");
        Destroy(gameObject);
        OnShardlingCollected?.Invoke();
    }

}
