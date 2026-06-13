using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject key;
    [SerializeField] protected Material outlineMaterial;

    protected SpriteRenderer spriteRenderer;
    protected Material originalMaterial;
    protected bool canBeInteracted = true;

    protected virtual void Start()
    {
        if (key != null)
        {
            key.SetActive(false);
        }

        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            this.spriteRenderer = spriteRenderer;
            originalMaterial = spriteRenderer.material;
        }
    }

    public virtual void Interact(Player player) { }
    public virtual void ShowKey()
    {
        if (key != null && canBeInteracted)
        {
            key.SetActive(true);
            spriteRenderer.material = outlineMaterial;
        }
    }

    public virtual void HideKey()
    {
        if (key != null)
        {
            key.SetActive(false);
            spriteRenderer.material = originalMaterial;
        }
    }
}
