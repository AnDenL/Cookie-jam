using UnityEngine;

public class HitAnimation : MonoBehaviour
{
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    [SerializeField] private string Particles;
    [SerializeField] private int Amount;
    [SerializeField] private Vector2 offset;

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
        if (!string.IsNullOrWhiteSpace(Particles))
            ParticleManager.PlayParticle(Particles, transform.position + (Vector3)offset, Amount);
        if (animator) animator.SetTrigger(HitHash);
    }

    private void OnDeath()
    {
        if (animator) animator.SetTrigger(DeathHash);
    }
}