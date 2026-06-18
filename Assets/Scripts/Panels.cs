using System.Collections.Generic;
using UnityEngine;

public class Panels : MonoBehaviour
{
    public static Dictionary<string, Panel> panels;

    private void Awake()
    {
        panels = new();
        foreach (Transform target in transform)
        {
            panels.Add(target.name, target.GetComponent<Panel>());
        }
    }
}