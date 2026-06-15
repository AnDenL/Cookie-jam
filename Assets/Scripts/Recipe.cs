using UnityEngine;

[CreateAssetMenu(fileName = "Create Recipe", menuName = "Objects/Recipe", order = 1)]
class Recipe : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public ItemStack[] RquiredObjects;
    public ItemStack ResultObjects;
}