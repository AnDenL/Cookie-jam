using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTableUI : MonoBehaviour
{
    public static CraftingTableUI instance;

    [SerializeField] private List<Recipe> recipes;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform recipesgrid;

    private List<RecipeSlot> recipeSlots = new();

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        foreach (var recipe in recipes)
        {
            recipeSlots.Add(Instantiate(recipeSlotPrefab, recipesgrid).GetComponent<RecipeSlot>());
            recipeSlots[^1].SetRecipe(recipe);
        }
    }

    public void UpdateUI(int index)
    {
        if (gameObject.activeInHierarchy)
        {
            foreach (var slot in recipeSlots)
                slot.UpdateSlot();
        }
        else
        {
            foreach (var slot in recipeSlots)
                slot.NeedsRebuild = true;
        }
    }
}
