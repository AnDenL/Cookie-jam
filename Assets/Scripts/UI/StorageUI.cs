using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    private static readonly int SelectedHash = Animator.StringToHash("Selected");

    [Header("Items")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform grid;

    private Inventory inventory;
    
    private List<ItemSlot> itemSlots = new();

    private int? itemToDrag = null;

    public void Open(Inventory inventory)
    {
        if (inventory == this.inventory) return;
        if (this.inventory != null)
        {
            this.inventory.OnSlotChange -= UpdateUI;
            this.inventory.OnNewSlot -= CreateSlot;
            this.inventory.SlotReinit -= ReinitSlot;
        }
        this.inventory = inventory;
        inventory.OnSlotChange += UpdateUI;
        inventory.OnNewSlot += CreateSlot;
        inventory.SlotReinit += ReinitSlot;

        foreach (var item in itemSlots)
        {
            Destroy(item.gameObject);
        }

        itemSlots = new();

        foreach (var item in inventory.items)
        {
            CreateSlot(item, 0);
        }
    }

    private void Update()
    {
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