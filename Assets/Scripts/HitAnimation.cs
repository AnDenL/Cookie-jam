using UnityEngine;

public class HitAnimation : MonoBehaviour
{
    [SerializeField] private string Particles;
    [SerializeField] private int Amount;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        var health = GetComponent<HealthBase>();

        health.OnHeal += OnHeal;
        health.OnHit += OnHit;
        health.OnDeath += OnDeath;
    }

    private void OnHeal(float value)
    {
        animator.SetTrigger("Heal");
    }

    private void OnHit(float value)
    {
        ParticleManager.PlayParticle(Particles, transform.position, Amount);
        animator.SetTrigger("Hit");
    }

    private void OnDeath()
    {
        animator.SetTrigger("Death");
    }
}