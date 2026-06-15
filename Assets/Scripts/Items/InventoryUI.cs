using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform grid;

    [Header("Craft")]
    [SerializeField] private List<Recipe> recipes;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform recipesgrid;

    private List<ItemSlot> itemSlots = new();
    private List<RecipeSlot> recipeSlots = new();

    private Inventory inventory => PlayerController.Player.Inventory;

    private void Start()
    {
        inventory.OnSlotChange += UpdateUI;
        inventory.OnNewSlot += CreateSlot;

        foreach (var item in inventory.items)
        {
            CreateSlot(item, 0);
        }

        foreach (var recipe in recipes)
        {
            recipeSlots.Add(Instantiate(recipeSlotPrefab, recipesgrid).GetComponent<RecipeSlot>());
            recipeSlots[^1].SetRecipe(recipe);
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
        itemSlots.Add(Instantiate(itemSlotPrefab, grid).GetComponent<ItemSlot>());
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

        if (inventoryPanel.activeInHierarchy)
        {
            foreach (var slot in recipeSlots)
                slot.UpdateUI();
        }
        else
        {
            foreach (var slot in recipeSlots)
                slot.NeedsRebuild = true;
        }

        itemSlots[index].UpdateUI();
    }
}