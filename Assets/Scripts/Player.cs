using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject corpse;
    [SerializeField]  private HealthBase health;

    private void Start()
    {
        health.OnDeath += OnDeath;
    }

    private void OnDeath()
    {
        health.GetComponent<Collider2D>().enabled = false;
        Instantiate(corpse, transform.position, Quaternion.identity);
    }
}
