using Creatures;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    public float Speed = 1;
    public float Noise = 1;

    private void Update()
    {
        var dir = new Vector3(Mathf.PerlinNoise1D(transform.position.x + Time.time), Mathf.PerlinNoise1D(transform.position.y + Time.time), 0).normalized;
        transform.position = Vector2.MoveTowards(transform.position, PlayerController.Player.transform.position + dir, Time.deltaTime * Speed);

        transform.position += Noise * Time.deltaTime * dir;
    }
}
