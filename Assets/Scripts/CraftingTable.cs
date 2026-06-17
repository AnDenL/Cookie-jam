using System.Collections;
using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class CraftingTable : Interactable
{
    private void Update()
    {
        if (Vector2.Distance(PlayerController.Player.transform.position, transform.position) > 5) 
            CraftingTableUI.instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CraftingTableUI.instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void Interact(Creature creature)
    {
        CraftingTableUI.instance.transform.GetChild(0).gameObject.SetActive(!CraftingTableUI.instance.transform.GetChild(0).gameObject.activeInHierarchy);
    }
}
