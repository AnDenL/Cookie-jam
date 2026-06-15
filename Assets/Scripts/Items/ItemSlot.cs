using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Creatures;

[RequireComponent(typeof(Animator))]
public class ItemSlot : MonoBehaviour
{
    private static readonly int OnSelectHash = Animator.StringToHash("OnSelect");
    public ItemStack itemStack;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip sound;

    private void Start() => animator = GetComponent<Animator>();

    public void SetItem(ItemStack newItem)
    {
        itemStack = newItem;
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count.ToString();
        UpdateUI();
    }

    public void UpdateUI()
    {
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count > 1 ? itemStack.Count.ToString() : string.Empty;
    }

    public void SelectAnimation()
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetTrigger(OnSelectHash);
        PlayerController.Player.PlaySound(sound);
    }

    public void Disabled(bool value)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetBool("Disabled", value);
    }
    
    public void Clear()
    {
        itemStack = null;
        image.sprite = null;
        image.enabled = false;
        countText.text = "";
    }
}