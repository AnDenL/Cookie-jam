using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    public Transform Target;
    
    private Rigidbody2D rb;
    private TargetJoint2D tj;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tj = GetComponent<TargetJoint2D>();
    }

    private void Update()
    {
        tj.target = Target.position;
    }
}
