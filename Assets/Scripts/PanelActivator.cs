using Creatures;
using UnityEngine;

public class PanelActivator : Interactable
{
    public string Name;

    private Panel panel;

    protected override void Start()
    {
        base.Start();

        panel = Panels.panels[Name];
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
        panel.gameObject.SetActive(!panel.gameObject.activeInHierarchy);
    }
}
