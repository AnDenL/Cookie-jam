using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform grid;

    private List<ItemSlot> itemSlots = new();

    private Inventory inventory => PlayerController.Player.Inventory;

    private void Start()
    {
        inventory.OnSlotChange += UpdateUI;
        inventory.OnNewSlot += CreateSlot;

        foreach (var item in inventory.items)
        {
            CreateSlot(item, 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);
        }
    }

    private void CreateSlot(ItemStack stack, int index)
    {
        itemSlots.Add(Instantiate(itemSlotPrefab, grid.transform).GetComponent<ItemSlot>());
        itemSlots[^1].SetItem(stack);
    }

    private void UpdateUI(int index)
    {
        if (itemSlots[index].itemStack.Count == 0)
        {
            Destroy(itemSlots[index].gameObject);
            itemSlots.RemoveAt(index);
            return;
        }
        itemSlots[index].UpdateUI();
    }
}