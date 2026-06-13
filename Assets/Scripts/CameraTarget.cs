using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTarget : MonoBehaviour
{
    [Range(1,10)]
    [SerializeField] private int targetWeight = 6;
    [SerializeField] private Transform target;

    private Camera cameraComponent;

    private void Start()
    {
        cameraComponent =  GetComponent<Camera>();
    }

    private void Update()
    {
        if (!Game.IsPaused && target != null)
        {
            Vector3 mousePosition =
                (cameraComponent.ScreenToWorldPoint(Input.mousePosition) - target.position)
                / targetWeight;

            transform.position = target.position + mousePosition;
        }
    }
}
