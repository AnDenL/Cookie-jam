using System.Collections;
using Creatures;
using TMPro;
using UnityEngine;

public class PHealthUI : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private ShakingText shakingText;

    private HealthBase health;
    private float final;
    private float hp;
    private bool anim;

    private void Start()
    {
        health = PlayerController.Player.HealthComponent;
        hp = health.Health / health.MaxHealth;
        material.SetFloat("_Health", hp); 
        health.OnHealthChange += OnHealthChange;
    }

    private void OnHealthChange(float health, float maxHealth)
    {
        hpText.text = "" + health;
        shakingText.enabled = health / maxHealth < 0.2f;
        final = health / maxHealth;
        anim = true;
    }

    private void Update()
    {
        if (anim == true)
        {
            hp = Mathf.Lerp(hp, final, Time.deltaTime * 2);
            material.SetFloat("_Health", hp); 

            if (hp == final) anim = false;
        }
    }

    private void OnDisable()
    {
        health.OnHealthChange -= OnHealthChange;
    }
}
