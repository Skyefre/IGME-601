using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour, ICollectible//, IDictionary
{
    public void Collect()
    {
       Debug.Log("You collected an item!");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
