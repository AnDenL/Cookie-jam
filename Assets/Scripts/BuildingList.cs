using UnityEngine;

[CreateAssetMenu(fileName = "BuildList", menuName = "Building/BuildList", order = -1000)]
public class BuildingList : ScriptableObject
{
    public BuildingPart[] buildings;
}
