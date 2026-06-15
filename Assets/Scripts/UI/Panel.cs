using Unity.VisualScripting;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private Vector2 startCursor;
    private Vector2 startPosition;
    private RectTransform rt;
    private Vector2 startSize;
    private Vector2 multiplayer;
    private float height;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        height = rt.rect.height / 2;
    }

    public void StartDrag()
    {
        startPosition = transform.position;
        startCursor = Input.mousePosition;
    }

    public void Drag()
    {
        transform.position = startPosition - (startCursor - (Vector2)Input.mousePosition);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, 25, Screen.width-25),
            Mathf.Clamp(transform.position.y, 0, Screen.height-height), 0);
    }
}
