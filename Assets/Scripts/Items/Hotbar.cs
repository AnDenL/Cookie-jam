using UnityEngine;
using System.Collections.Generic;
using Creatures;

public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Item hand;

    private List<Item> hotbarItems = new();
    private List<ItemSlot> hotbarItemSlots = new();
    private Dictionary<int, ItemSlot> hotbarSlots = new();
    private int selected = 0; 

    private Inventory inventory => PlayerController.Player.Inventory;

    private void Start()
    {
        instance = this;
        hotbarSlots = new();
        CreateSlot(new ItemStack(hand, 1), -1);
        inventory.OnSlotChange += UpdateUI;
        inventory.OnNewSlot += CreateSlot;
    }

    private void Update()
    {
        if (!PlayerController.Player.CanAct) return;

        if (Input.GetMouseButtonDown(0)) 
        {
            if (hotbarItems[selected].Use(PlayerController.Player) && hotbarItems[selected].Consumable) 
            {
                if (inventory.GetItemCount(hotbarItems[selected]) == 1)
                {
                    inventory.RemoveItem(hotbarItems[selected], 1);
                    selected--; 
                    return;
                }
                else inventory.RemoveItem(hotbarItems[selected], 1);
            }
        }

        else if (Input.mouseScrollDelta.y != 0)
        {
            int direction = Input.mouseScrollDelta.y > 0 ? -1 : 1;
            ScrollHotbar(direction);
        }
        hotbarItems[selected].WhileSelected(PlayerController.Player);
    }

    private void ScrollHotbar(int direction)
    {
        int previousSelected = selected;
        selected += direction;
        if (selected < 0) selected = hotbarItems.Count - 1;
        else if (selected > hotbarItems.Count - 1) selected = 0;
        if (previousSelected != selected)
        {
            hotbarItemSlots[selected].SelectAnimation();
            Hints.Instance.ShowHint(hotbarItems[selected].Name, 1, AnimationCurve.Linear(0,1,1,0));
            hotbarItems[previousSelected].Deselect(PlayerController.Player);
            hotbarItems[selected].Select(PlayerController.Player);
        }
    }

    private void UpdateUI(int index)
    {
        if (!hotbarSlots.ContainsKey(index)) return;
        if (hotbarSlots[index].itemStack.Count == 0)
        {
            Destroy(hotbarSlots[index].gameObject);
            hotbarItems.Remove(hotbarSlots[index].itemStack.Item);
            hotbarItemSlots.Remove(hotbarSlots[index]);
            hotbarSlots.Remove(index);
            return;
        }
        hotbarSlots[index].UpdateUI();
    }

    private void CreateSlot(ItemStack stack, int index)
    {
        if (stack.Item.Type == ItemType.Active)
        {
            var slot = Instantiate(itemSlotPrefab, transform.GetChild(0)).GetComponent<ItemSlot>();
            hotbarSlots.Add(index, slot);
            hotbarItems.Add(stack.Item);
            hotbarItemSlots.Add(slot);
            hotbarSlots[index].SetItem(stack);

            if(hotbarItems.Count == 1) hotbarItems[0].Select(PlayerController.Player);
        }
    }
}