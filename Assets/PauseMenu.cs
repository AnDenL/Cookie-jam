using System.Collections;
using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerController.CanInteract = panel.activeInHierarchy;
            Time.timeScale = panel.activeInHierarchy ? 1 : 0;
            panel.SetActive(!panel.activeInHierarchy);
        }
    }
}
