using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTarget : MonoBehaviour
{
    [Range(1,10)]
    [SerializeField] private int speed = 6;
    [SerializeField] private Transform target;
    

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, target.position, speed * Time.deltaTime);
    }
}
