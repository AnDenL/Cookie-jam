using Creatures;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public static Minimap Instance;
    public Image Map, Fullmap;
    public Camera RenderCamera;
    public bool IsOpened;
    
    private float currentZoom = 4;
    private Vector2 fullMapPos;
    private Vector2 startDragMouse, startDragPos;

    private Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        Map.transform.localScale = Vector2.one * currentZoom;
        Fullmap.transform.localScale = Map.transform.localScale * 4;
        //Set(new Vector2Int(450, 450));
    }

    private void Switch()
    {
        IsOpened = !IsOpened;

        animator.SetBool("Opened", IsOpened);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) Switch();
        if (IsOpened)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragMouse = Input.mousePosition;
                startDragPos = fullMapPos;
            }
            else if (Input.GetMouseButton(0))
            {
                fullMapPos = startDragPos + ((Vector2)Input.mousePosition - startDragMouse);
            }

            Fullmap.transform.position = fullMapPos;
        }

        Map.transform.localPosition = -PlayerController.Player.transform.position * currentZoom;
    }

    public void Zoom()
    {
        if (Input.mouseScrollDelta.y == 0) return;

        float previousZoom = currentZoom;

        currentZoom += Input.mouseScrollDelta.y > 0 ? 0.5f : -0.5f;
        currentZoom = Mathf.Clamp(currentZoom, 1, 8);
        Map.transform.localScale = Vector2.one * currentZoom;
        Fullmap.transform.localScale = Map.transform.localScale * 4;

        Vector2 halfScreen = new Vector2(Screen.width, Screen.height) / 2;

        fullMapPos = (fullMapPos -halfScreen) / previousZoom * currentZoom;
        fullMapPos += halfScreen;
        Fullmap.transform.position = fullMapPos;
    }
}