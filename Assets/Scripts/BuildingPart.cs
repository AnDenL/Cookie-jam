using UnityEngine;

[CreateAssetMenu(fileName = "BuildPart", menuName = "Building/BuildingPart", order = -1000)]
public class BuildingPart : ScriptableObject
{
    public string Name;
    public Sprite icon;
    public GameObject prefab;
    public BuildingLayer layer;
    public ItemStack[] requiredMaterials;
    public Vector2Int size = Vector2Int.one;

    public Vector2Int[] GetOccupiedCells(Vector2Int origin)
    {
        Vector2Int[] result = new Vector2Int[size.x * size.y];
        int index = 0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                result[index++] = origin + new Vector2Int(x, y);
            }
        }
        return result;
    }
}

public enum BuildingLayer
{
    Floor,
    Wall,
    All
}