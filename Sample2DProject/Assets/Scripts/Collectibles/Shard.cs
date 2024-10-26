using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour, ICollectible
{

    public static event Action OnShardCollected;
    public void Collect()
    {
        Debug.Log("You collected an item!");
        Destroy(gameObject);
        OnShardCollected?.Invoke();
    }

}
