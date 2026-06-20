using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Interactable
{
    public override void Interact(Creature creature)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    } 
}
