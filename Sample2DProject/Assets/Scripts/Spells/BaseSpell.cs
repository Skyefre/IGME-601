using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpell : MonoBehaviour
{
    public abstract void UseSpell();
    public abstract void UseChargedSpell();
}
