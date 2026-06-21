using UnityEngine;

public class Death : MonoBehaviour
{
    public HealthBase health;
    public GameObject ui;

    private void Start()
    {
        health.OnDeath += Restart;
    }

    private void Restart()
    {
        ui.SetActive(true);
    }
}