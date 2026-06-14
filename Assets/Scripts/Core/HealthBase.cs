using System;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [SerializeField] protected float health;
    public float Health => health;
    [SerializeField] protected float maxHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] protected bool isDead;
    public bool IsDead => isDead;

    public Action<float> OnHit;
    public Action<float> OnHeal;
    public Action<float,float> OnHealthChange;
    public event Action OnDeath;

    protected void Start()
    {
        maxHealth = health;
    }

    public void TakeHit(float damage)
    {
        health -= damage;

        OnHit?.Invoke(damage);
        OnHealthChange?.Invoke(health,maxHealth);

        if (health <= 0)
        {
            Death();
        }
    }

    public void Heal(float value)
    {
        health += value;

        OnHeal?.Invoke(value);
        OnHealthChange?.Invoke(health,maxHealth);

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    private void Death()
    {
        OnDeath?.Invoke();
        isDead = true;
    }
}
