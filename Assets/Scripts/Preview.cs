using UnityEngine;

public class Preview : MonoBehaviour
{
    public static SpriteRenderer Instance;

    private void Awake() => Instance = GetComponent<SpriteRenderer>();
}