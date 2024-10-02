using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : BaseSpell
{
    public GameObject IceBlock;
    public override void UseChargedSpell()
    {
        Instantiate(IceBlock ,this.transform);
    }

    public override void UseSpell()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
