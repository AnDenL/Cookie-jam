using System.Collections;
using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class CraftingTable : Interactable
{
    private void Update()
    {
        if (Vector2.Distance(PlayerController.Player.transform.position, transform.position) > 5) 
            CraftingTableUI.instance.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CraftingTableUI.instance.gameObject.SetActive(false);
    }

    public override void Interact(Creature creature)
    {
        CraftingTableUI.instance.gameObject.SetActive(!CraftingTableUI.instance.gameObject.activeInHierarchy);
    }
}
