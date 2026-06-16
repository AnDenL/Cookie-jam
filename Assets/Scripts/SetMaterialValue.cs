using UnityEngine;

public class SetMaterialValue : MonoBehaviour
{
    [SerializeField] private string property;
    [SerializeField] private Material material;

    public void Set(float value) => material.SetFloat(property, value);
}
