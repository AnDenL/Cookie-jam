using System.Collections;
using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class Temperature : MonoBehaviour
{
    public int t = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Temperature += t;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Temperature -= t;
        }
    }
}
