using UnityEngine;
using UnityEngine.EventSystems;

public class Panel : MonoBehaviour
{
    private Vector2 startCursor;
    private Vector2 startPosition;

    public void StartDrag()
    {
        startPosition = transform.position;
        startCursor = Input.mousePosition;
    }

    public void Drag()
    {
        transform.position = startPosition + (startCursor - (Vector2)Input.mousePosition);
    }
}
