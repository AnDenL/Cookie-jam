using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Creatures;
using Unity.VisualScripting;
using System;

[RequireComponent(typeof(Animator))]
public class ItemSlot : MonoBehaviour
{
    private static readonly int OnSelectHash = Animator.StringToHash("OnSelect");
    public ItemStack itemStack;
    public int ItemCount => PlayerController.Player.Inventory.GetItemCount(itemStack.Item);

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private AudioClip sound;
    
    public Animator animator;

    private void Start() => animator = GetComponent<Animator>();

    public void SetItem(ItemStack newItem)
    {
        itemStack = newItem;
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count.ToString();
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count > 1 ? itemStack.Count.ToString() : string.Empty;
    }

    public void HorbarUpdate()
    {
        countText.text = ItemCount > 1 ? itemStack.Count.ToString() : string.Empty;
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

    public void OnClick()
    {
        int my_id = transform.GetSiblingIndex();
        Hotbar.instance.SelectItem(my_id);
    }

    public void OnDragStart()
    {
        print("OnDragStart");
        int my_id = transform.GetSiblingIndex();
        InventoryUI.Instance.DragStart(my_id);
    }

    public void OnDragEnd()
    {
        print("OnDragEnd");
        int my_id = transform.GetSiblingIndex();
        InventoryUI.Instance.Rearrange(my_id);
    }
}