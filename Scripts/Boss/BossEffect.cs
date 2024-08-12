using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffect : MonoBehaviour
{
    private int atk;
    public int Atk { get => atk; set => atk = value; }

    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Health player = other.GetComponent<Health>();
            if (player != null)
            {
                player.Hit(Atk);
            }
        }
    }
}
