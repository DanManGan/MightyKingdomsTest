using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;
    public float health = 100f;
    public ParticleSystem hitParticles;



    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        health = health - damage;
        hitParticles.transform.position = hitPoint;
        hitParticles.Play();

        if (health <= 0) {
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
