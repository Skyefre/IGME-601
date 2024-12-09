using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardCounter : MonoBehaviour
{
    public List<GameObject> Shards;
    public int shardCounter;
    // Start is called before the first frame update
    void Start()
    {
        shardCounter = 0;
        Shards = new List<GameObject>();


    }

    public void AddShard()
    {
        if(Shards.Count <=0)
        {
            foreach (Transform child in transform)
            {
                Shards.Add(child.gameObject);
            }
        }
        Debug.Log("Called");
        shardCounter++;
        if(shardCounter<= Shards.Count) {
            Debug.Log(Shards[shardCounter - 1]);
            Shards[shardCounter - 1].GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 1);
        }
        
    }
    
}
