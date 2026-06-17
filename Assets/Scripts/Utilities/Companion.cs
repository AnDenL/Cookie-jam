using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    public Transform Target;
    
    private TargetJoint2D tj;

    private void Start()
    {
        tj = GetComponent<TargetJoint2D>();
    }

    private void Update()
    {
        var dir = Target.position - (Target.position - transform.position).normalized;
        tj.target = dir;

        if (dir.x > transform.position.x)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else if (dir.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }
}
