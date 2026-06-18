using System.Collections.Generic;
using Creatures;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static readonly int SelectedHash = Animator.StringToHash("Selected");
    public static InventoryUI Instance;

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

    private int? itemToDrag = null;

    private void Start()
    {
        Instance = this;
        inventory.OnSlotChange += UpdateUI;
        inventory.OnNewSlot += CreateSlot;
        inventory.SlotReinit += ReinitSlot;

        recipesgrid.gameObject.SetActive(false);
        recipesgrid.gameObject.SetActive(true);

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

        if (Input.GetMouseButtonUp(0))
        {
            if (itemToDrag != null)
                itemSlots[(int)itemToDrag].animator.SetBool(SelectedHash, false);
            if (!Game.HoverUI())
            {   
                var dir = Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - PlayerController.Player.transform.position;
                Drop(dir.normalized);
            }
        }
    }

    private void CreateSlot(ItemStack stack, int index)
    {
        itemSlots.Add(Instantiate(itemSlotPrefab, grid).GetComponent<ItemSlot>());
        itemSlots[^1].SetItem(stack);
    }

    private void ReinitSlot(ItemStack stack, int index)
    {
        print("id "+ index + " : Item " + stack.Item.Name);
        itemSlots[index].SetItem(stack);
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
                slot.UpdateSlot();
        }
        else
        {
            foreach (var slot in recipeSlots)
                slot.NeedsRebuild = true;
        }

        itemSlots[index].UpdateSlot();
    }

    public void DragStart(int id)
    {
        if (itemToDrag != null)
            itemSlots[(int)itemToDrag].animator.SetBool(SelectedHash, false);
        itemToDrag = id;
        itemSlots[id].animator.SetBool(SelectedHash, true);
    }

    public void Drop(Vector2 dir)
    {
        if (itemToDrag == null) return;

        int id = (int)itemToDrag;
        ItemStack stack = inventory.GetStackById(id);
        stack = new(stack.Item, stack.Count);

        var i = Instantiate(Game.GlobalObjects[0], PlayerController.Player.transform);
        i.transform.parent = null;
        i.GetComponent<ItemPickUp>().Set(stack);
        i.GetComponent<ArcAnim>().DropTo(PlayerController.Player.transform.position + (Vector3)dir);

        inventory.RemoveStack(id);

        itemToDrag = null;
    }

    public void Rearrange(int id)
    {
        if (itemToDrag == null) return;
        if (itemToDrag == id)
        {
            itemToDrag = null;
            return;
        }
        itemSlots[(int)itemToDrag].animator.SetBool(SelectedHash, false);
        itemSlots[id].SelectAnimation();    

        inventory.Swap((int)itemToDrag, id);
        itemToDrag = null; 
    }
}