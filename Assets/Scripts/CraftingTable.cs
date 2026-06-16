using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : Interactable
{
    public override void Interact(Creature creature)
    {
        CraftingTableUI.instance.gameObject.SetActive(!CraftingTableUI.instance.gameObject.activeInHierarchy);
    }
}
