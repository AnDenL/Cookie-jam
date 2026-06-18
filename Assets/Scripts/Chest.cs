using Creatures;
using UnityEngine;

public class Chest : Interactable
{
    public string Name;
    public Creature owner;

    private StorageUI storageUI;
    private Panel panel;

    protected override void Start()
    {
        base.Start();

        panel = Panels.panels[Name];
        storageUI = panel.GetComponent<StorageUI>();
    }

    private void Update()
    {
        if (Vector2.Distance(PlayerController.Player.transform.position, transform.position) > 5) 
        {
            panel.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        panel.gameObject.SetActive(false);
    }

    public override void Interact(Creature creature)
    {
        if (!creature.Controller.IsPlayer) return;
        panel.gameObject.SetActive(!panel.gameObject.activeInHierarchy);
        storageUI.Open(owner.Inventory);
    }
}
