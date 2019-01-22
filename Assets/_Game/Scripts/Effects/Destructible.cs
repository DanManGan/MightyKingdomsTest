using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health = health - damage;

        if (health <= 0) {
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
