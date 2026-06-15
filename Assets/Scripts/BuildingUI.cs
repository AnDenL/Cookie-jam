using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject requirementsPanel;
    
    private TextMeshProUGUI[] requirementsCounts = new TextMeshProUGUI[4];
    private BuildingPart lastPart;
    private Animator animator;

    public Image[] requirementsImages = new Image[4];
    
    private void Awake()
    {
        Instance = this;
        animator = transform.GetChild(1).GetComponent<Animator>();

        for (int i = 0; i < requirementsImages.Length; i++)
            requirementsCounts[i] = requirementsImages[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();

        PlayerController.Player.Inventory.OnInventoryChange += () => UpdateRequirments(lastPart);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildingPanel.SetActive(!buildingPanel.activeInHierarchy);
        } 
    }
    
    public void SetAnimatorState(bool state)
    {
        animator.SetBool("Building", state);
    }

    public void UpdateRequirments(BuildingPart part)
    {
        if (requirementsPanel.activeInHierarchy == false) return;

        lastPart = part;
        for (int i = 0; i < part.requiredMaterials.Length; i++)
        {
            requirementsImages[i].gameObject.SetActive(true);
            requirementsImages[i].sprite = part.requiredMaterials[i].Item.Icon;

            int haveCurrent = PlayerController.Player.Inventory.GetItemCount(part.requiredMaterials[i].Item);
            int needCurrent = part.requiredMaterials[i].Count;
            requirementsCounts[i].color = haveCurrent >= needCurrent ? Color.white : Color.red;
            requirementsCounts[i].text = haveCurrent.ToString() + " / " + needCurrent.ToString();
        }
        for (int i = part.requiredMaterials.Length; i < requirementsImages.Length; i++)
        {
            requirementsImages[i].gameObject.SetActive(false);
        }
    }
}
