using Creatures;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public float heal;

    public void Update()
    {
        if (Vector2.Distance(PlayerController.Player.transform.position, transform.position) < 6)
        {
            PlayerController.Player.HealthComponent.HealthEditable += heal * Time.deltaTime;
        }
    }
}