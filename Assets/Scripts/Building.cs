using System.Collections.Generic;
using Creatures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    static public BuildingList buildList;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject BuildCell;
    [SerializeField] private BuildingList list;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private LayerMask layer;

    private Transform selectedPart;
    private float buildTime;
    private float hintTime;
    private enum BuildingState { Choose, Placing }
    private BuildingState state = BuildingState.Choose;

    private void Start()
    {
        buildList = list;

        //avalible buildCell
        if (transform.childCount != buildList.buildings.Length)
        {
            foreach (BuildingPart building in buildList.buildings)
            {
                GameObject cell = Instantiate(BuildCell, transform.position, Quaternion.identity);
                cell.transform.SetParent(transform, false);
                cell.transform.GetChild(0).GetComponent<Image>().sprite = building.icon;
            }
            for (int i = 0; i < buildList.buildings.Length; i++) transform.GetChild(i).name = i.ToString();
        }
    }
    
    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (state == BuildingState.Placing && hintTime < Time.time)
        {
            Hints.Instance.ShowHint("Left mouse button to place, right mouse button to cancel", 2f);
            hintTime = Time.time + 2f;
        }
        if (state == BuildingState.Choose)
        {
            CheckHover();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (state == BuildingState.Choose)
                TrySelectPart();
            else if (state == BuildingState.Placing)
                TryPlaceBuilding();
        }
        if (Input.GetMouseButton(0) && buildTime < Time.time && state == BuildingState.Placing)
        {
            TryPlaceBuilding();
        }
        else if (Input.GetMouseButtonDown(1) && state == BuildingState.Placing)
        {
            Hotbar.instance.enabled = true;
            PlayerController.Player.CanAct = false;
            Preview.Instance.gameObject.SetActive(false);
            state = BuildingState.Choose;
            buildingUI.SetAnimatorState(false);
        }
    }

    private void TrySelectPart()
    {
        Preview.Instance.gameObject.SetActive(false);
        selectedPart = RaycastUI(Input.mousePosition)?.GetComponent<RectTransform>();

        if (selectedPart == null) return;

        if (int.TryParse(selectedPart.name, out int index))
        {
            buildingUI.SetAnimatorState(true);
            BuildingPart part = buildList.buildings[index];

            Preview.Instance.gameObject.SetActive(true);
            Preview.Instance.sprite = part.prefab.GetComponent<SpriteRenderer>().sprite;
            state = BuildingState.Placing;

            buildingUI.UpdateRequirments(part);
            buildTime = Time.time + 1f;
            Hotbar.instance.enabled = false;
            PlayerController.Player.CanAct = true;
        }
    }

    private void CheckHover()
    {
        Preview.Instance.gameObject.SetActive(false);
        selectedPart = RaycastUI(Input.mousePosition)?.GetComponent<RectTransform>();

        if (selectedPart == null) return;
        if (int.TryParse(selectedPart.name ,out int index))
        {
            BuildingPart part = buildList.buildings[index];
            buildingUI.UpdateRequirments(part);
        }
    }

    private void TryPlaceBuilding()
    {
        if (!int.TryParse(selectedPart.name, out int index)) return;
        BuildingPart part = buildList.buildings[index];
        bool spaceIsEmpty = Physics2D.Raycast(Preview.Instance.transform.position, Vector2.zero, 0.1f, layer).collider == null;

        if (spaceIsEmpty)
        {
            if (EnoughMaterials(part))
            {
                
            }
            else Hints.Instance.ShowHint("Not enough materials", 0.6f);
        }
    }

    private void OnDisable()
    {
        if (gameObject.scene.isLoaded) Preview.Instance.gameObject.SetActive(false);
        state = BuildingState.Choose;
        Hotbar.instance.enabled = true;
    }

    private GameObject RaycastUI(Vector2 screenPosition)
    {
        var pointerData = new PointerEventData(eventSystem) { position = screenPosition };
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (int.TryParse( result.gameObject.name,out int index))
                return result.gameObject;
        }

        return null;
    }

    private Vector3Int ToTilemapPosition(Vector3 worldPos)
    {
        return new Vector3Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y), 0);
    }
    private bool EnoughMaterials(BuildingPart part)
    {
        foreach (ItemStack material in part.requiredMaterials)
        {
            if (PlayerController.Player.Inventory.GetItemCount(material.Item) < material.Count)
            {
                return false;
            }
        }
        return true;
    }
}
