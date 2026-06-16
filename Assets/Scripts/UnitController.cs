using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public static UnitController instance;

    public List<Creature> units;

    public void Awake()
    {
        instance = this;
    }
}