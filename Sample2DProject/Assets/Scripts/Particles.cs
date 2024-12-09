using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField] ParticleSystem deathParticle;

     public void PlayDeathParticles(Vector2 pos)
     {
        Debug.Log("PARTICLESSSS");
        deathParticle.Play();
     }
}
