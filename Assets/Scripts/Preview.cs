using UnityEngine;

public class Preview : MonoBehaviour
{
    public static SpriteRenderer Instance;

    private void Awake() => Instance = GetComponent<SpriteRenderer>();

    private void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = RoundToGrid(mouseWorld);
    }

    private Vector3 RoundToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0f);
    }
}