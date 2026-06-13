using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private float defaultAlpha = 1, overlapAlpha = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var sr in spriteRenderers)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, overlapAlpha);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var sr in spriteRenderers)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, defaultAlpha);
        }
    }
}
