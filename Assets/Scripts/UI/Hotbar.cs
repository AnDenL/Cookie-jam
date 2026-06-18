using UnityEngine;
using System.Collections.Generic;
using Creatures;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Rendering.Universal;

public class Hotbar : MonoBehaviour
{
    private static readonly int SelectedHash = Animator.StringToHash("Selected");
    public static Hotbar instance;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Item hand;
    [SerializeField] private SpriteRenderer itemPreview;

    private Light2D itemLight;

    private List<Item> hotbarItems = new();
    private List<ItemSlot> hotbarSlots = new();
    private int selected = 0; 

    private readonly KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    private Inventory inventory => PlayerController.Player.Inventory;

    private void Start()
    {
        instance = this;
        hotbarSlots = new();

        itemLight = itemPreview.transform.GetChild(0).GetComponent<Light2D>();

        CreateSlot(new ItemStack(hand, 1), -1);

        inventory.OnItemChanged += UpdateUI;
        inventory.OnNewSlot += CreateSlot;
    }

    private void Update()
    {
        if (!PlayerController.Player.CanAct || Game.HoverUI()) return;

        for(int i = 0 ; i < keyCodes.Length ; ++i)
        {
            if(Input.GetKeyDown(keyCodes[i]))
            {
                SelectItem(i);
            }
        }


        if (selected != 0)
        {
            itemPreview.enabled = true  ;
            itemPreview.sprite = hotbarItems[selected].Icon;
            if (hotbarItems[selected].Light > 0) 
            {
                itemLight.intensity = hotbarItems[selected].Light;
                itemLight.enabled = true;
            }
            else itemLight.enabled = false;

            itemPreview.transform.position = PlayerController.Player.transform.position + 
                (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - PlayerController.Player.transform.position).normalized;
        }
        else 
        {
            itemPreview.enabled = false;
            itemLight.enabled = false;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            if (hotbarItems[selected].Use(PlayerController.Player) && hotbarItems[selected].Consumable) 
            {
                //print(inventory.GetItemCount(hotbarItems[selected]));
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
            hotbarSlots[selected].animator.SetBool(SelectedHash, true);

            hotbarSlots[previousSelected].animator.SetBool(SelectedHash, false);
            hotbarSlots[selected].SelectAnimation();
            Hints.Instance.ShowHint(hotbarItems[selected].Name, 1, AnimationCurve.Linear(0,1,1,0));
            hotbarItems[previousSelected].Deselect(PlayerController.Player);
            hotbarItems[selected].Select(PlayerController.Player);
        }
    }

    public void SelectItem(int id)
    {
        int previousSelected = selected;
        if (id >= 0 && id < hotbarItems.Count) selected = id;
        else return;

        hotbarSlots[selected].animator.SetBool(SelectedHash, true);

        hotbarSlots[previousSelected].animator.SetBool(SelectedHash, false);
        hotbarSlots[selected].SelectAnimation();
        Hints.Instance.ShowHint(hotbarItems[selected].Name, 1, AnimationCurve.Linear(0,1,1,0));
        hotbarItems[previousSelected].Deselect(PlayerController.Player);
        hotbarItems[selected].Select(PlayerController.Player);
    }

    private void UpdateUI(Item item)
    {
        var id = hotbarItems.FindIndex(s => s.Id == item.Id);

        if (id == -1)
            return;
        
        if (hotbarSlots[id].ItemCount == 0)
        {
            Destroy(hotbarSlots[id].gameObject);
            hotbarSlots.RemoveAt(id);
            hotbarItems.RemoveAt(id);
            return;
        }

        hotbarSlots[id].HorbarUpdate();
    }

    private void CreateSlot(ItemStack stack, int index)
    {
        if (stack.Item.Type == ItemType.Active)
        {
            var id = hotbarItems.FindIndex(s => s.Id == stack.Item.Id);

            if (id != -1)
                return;

            var slot = Instantiate(itemSlotPrefab, transform.GetChild(0)).GetComponent<ItemSlot>();
            hotbarSlots.Add(slot);
            hotbarItems.Add(stack.Item);
            hotbarSlots[^1].SetItem(stack);

            if (hotbarItems.Count == 1) hotbarItems[0].Select(PlayerController.Player);
        }
    }
}