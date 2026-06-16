using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.001f);
    }
}
