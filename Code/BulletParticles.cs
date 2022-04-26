using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Boss"))
        {
            FindObjectOfType<BossAI>().TakeDamage(10);
        }
    }
}
